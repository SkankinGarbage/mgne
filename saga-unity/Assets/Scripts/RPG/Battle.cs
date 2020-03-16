using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Battle model class for an in-progress fite
/// </summary>
public class Battle {

    public Party Player { get; private set; }
    public Party Enemy { get; private set; }
    public BattleView View { get; private set; }
    public string BgmKey { get; private set; }

    public bool IsDone { get; private set; }
    public bool IsVictory => !Enemy.IsAnyAlive;
    public bool IsDefeat => !Player.IsAnyAlive;

    private Intent[] playerIntents;
    private List<Intent> allIntents;
    private Dictionary<Unit, List<EffectDefend>> defenses;
    private Dictionary<Unit, MutationManager> mutationManagers;
    private List<TempStats> boosts;

    private Party BackupPlayer, BackupEnemy;

    public Battle(Party enemy, string bgmKey) {
        this.BgmKey = bgmKey;
        Player = Global.Instance().Party;
        Initialize(enemy);
    }

    private void Initialize(Party enemy) {
        Enemy = enemy;
        boosts = new List<TempStats>();
        defenses = new Dictionary<Unit, List<EffectDefend>>();
        mutationManagers = new Dictionary<Unit, MutationManager>();

        BackupPlayer = Player.Copy();
        BackupEnemy = Enemy.Copy();
    }

    #region Model

    public List<EffectDefend> GetDefensesForUnit(Unit unit) {
        if (defenses.ContainsKey(unit)) {
            return defenses[unit];
        } else {
            return new List<EffectDefend>();
        }
    }

    public MutationManager GetMutationManagerForUnit(Unit unit) {
        if (!mutationManagers.ContainsKey(unit)) {
            mutationManagers[unit] = new MutationManager(unit, this);
        }
        return mutationManagers[unit];
    }

    public void AddBoost(Unit target, StatSet boost, bool oneRoundOnly = false) {
        boosts.Add(new TempStats(target, boost, oneRoundOnly = false));
    }

    public void AddDefense(Unit defender, EffectDefend defense) {
        List<EffectDefend> defenseList;
        if (defenses.ContainsKey(defender)) {
            defenseList = defenses[defender];
        } else {
            defenseList = new List<EffectDefend>();
            defenses[defender] = defenseList;
        }
        defenseList.Add(defense);
    }

    /// <returns>True if the action was canceled</returns>
    public bool CancelAction(Unit target) {
        foreach (var intent in allIntents) {
            if (intent.Actor == target && !intent.IsFinished) {
                intent.Targets.Clear();
                return true;
            }
        }
        return false;
    }

    public async Task CheckDeathAsync(Unit target, bool silent = false) {
        if (!target.IsDead) return;
        View.UpdateForMemberChange();
        if (!silent) await WriteLineAsync(BattleBox.Tab + target.Name + " is defeated.");
    }

    /// <returns>True if got away successfully</returns>
    private bool TryEscapeChance() {
        return true; // lol
    }

    public void Reset() {
        IsDone = false;
        View.PopulateForFightRun();
        Player = BackupPlayer.Copy();
        Global.Instance().Data.Party = Player;
        Enemy = BackupEnemy.Copy();
        View.Populate(this);
        View.PlayBattleBGM();
    }

    #endregion

    #region Control flow

    public IEnumerator BattleRoutine(BattleView view) {
        View = view;
        yield return CoUtils.TaskRoutine(BattleAsync());
    }

    private async Task BattleAsync() {
        while (!IsDone) {
            View.battlebox.Clear();
            InitializeIntents();
            View.PopulateForFightRun();
            string fightRunCommand = null;
            do {
                fightRunCommand = await View.fightRunMenu.SelectCommandAsync();
            } while (fightRunCommand == null);
            switch (fightRunCommand) {
                case "FIGHT":
                    await FightAsync();
                    break;
                case "RUN":
                    await RunAsync();
                    break;
            }
            if (!IsDone) {
                await UpdateForEndOfRoundAsync();
            } else {
                await EndCombatAsync();
            }
        }
    }

    /// <returns>True if succeeded, false if canceled</returns>
    private async Task<bool> FightAsync() {
        var unitIndex = AdvanceUnitIndex(-1, 1);
        while (unitIndex < Player.Size && unitIndex >= 0) {
            var succeeded = await ConstructIntentForPlayerAsync(playerIntents[unitIndex]);
            unitIndex = AdvanceUnitIndex(unitIndex, succeeded ? 1 : -1);
        }
        if (unitIndex < 0) {
            return false;
        }

        allIntents = playerIntents.ToList();
        foreach (var enemy in Enemy) {
            if (enemy.CanAct) {
                var intent = await ConstructIntentForEnemyAsync(enemy);
                allIntents.Add(intent);
            }
        }
        allIntents.Sort(new Comparison<Intent>((a, b) => b.Priority - a.Priority));
        foreach (var intent in allIntents) {
            intent.OnRoundStart();
        }

        await ResolveIntentsAsync();
        await Global.Instance().Input.AwaitConfirm();

        return true;
    }

    private async Task UpdateForEndOfRoundAsync() {
        allIntents.Clear();
        defenses.Clear();
        var toClear = boosts.Where(boost => boost.IsOneRoundOnly);
        foreach (var boost in toClear) {
            boost.Decombine();
            boosts.Remove(boost);
        }

        var participants = new List<Unit>();
        participants.AddRange(Enemy);
        participants.AddRange(Player);
        foreach (var unit in participants) {
            await unit.UpdateForEndOfRoundAsync(this);
            if (IsVictory || IsDefeat) {
                IsDone = true;
                break;
            }
        }
    }

    private async Task EndCombatAsync() {
        foreach (var boost in boosts) {
            boost.Decombine();
        }
        boosts.Clear();

        foreach (var unit in Player) {
            unit.Status?.UpdateForEndOfCombat(unit);
        }

        if (IsVictory) {
            View.PlayVictoryBGM();
            await WriteLineAsync("");
            await WriteLineAsync(Player.Leader.Name + " is victorious.");
            await WriteLineAsync("");

            int gp = Enemy.Select(unit => unit.GP).Aggregate((x, y) => x + y);
            await WriteLineAsync("Found " + gp + " gold pieces.");
            Global.Instance().Data.AddGP(gp);
            await Global.Instance().Input.AwaitConfirm();

            foreach (var unit in Player) {
                await DoMutationsForUnitAsync(unit);
            }
            await DoMeatAsync();
            await View.CloseRoutine();
        } else if (IsDefeat) {
            View.PlayDefeatBGM();
            await WriteLineAsync("");
            await WriteLineAsync("");
            await WriteLineAsync(Player.Leader.Name + " is defeated...");
            await Global.Instance().Input.AwaitConfirm();
            await View.retry.RetryAsync(this);
        }
    }

    /// <returns>True if succeeded, false if canceled</returns>
    private async Task<bool> ConstructIntentForPlayerAsync(Intent intent) {
        View.PopulateForUnitIntentSelect(intent);
        var selector = View.inventory.Selector;
        selector.Selection = intent.FindIndexForItem();
        do {
            var slot = await selector.SelectItemAsync(null, true);
            selector.ClearSelection();
            if (slot < 0) {
                return false;
            }
            intent.SetItem(intent.Actor.Equipment[slot]);
        } while (intent.Item == null || !intent.Item.IsBattleUseable);

        var targets = await intent.Item.Effect.AcquireTargetsAsync(intent.Actor, this, false);
        if (targets == null) {
            return await ConstructIntentForPlayerAsync(intent);
        }

        intent.Targets.AddRange(targets);
        return true;
    }

    private async Task ResolveIntentsAsync() {
        foreach (var intent in allIntents) {
            await intent.ResolveAsync();
            await WriteLineAsync("");

            if (IsVictory || IsDefeat) {
                IsDone = true;
                break;
            }
        }
    }

    private async Task RunAsync() {
        if (TryEscapeChance()) {
            await WriteLineAsync();
            await WriteLineAsync("Escaped!", true);
            IsDone = true;
        }
    }

    private void InitializeIntents() {
        playerIntents = new Intent[Player.Size];
        for (var i = 0; i < Player.Size; i += 1) {
            playerIntents[i] = new Intent(Player[i], this);
        }
    }

    private Task<Intent> ConstructIntentForEnemyAsync(Unit enemy) {
        return enemy.AI.ConstructIntentAsync(this);
    }

    private int AdvanceUnitIndex(int index, int delta) {
        do {
            index += delta;
        } while (index < Player.Size && index >= 0 && !Player[index].CanAct);
        return index;
    }

    private async Task DoMutationsForUnitAsync(Unit unit) {
        if (!unit.IsAlive || unit.Race != Race.MUTANT || UnityEngine.Random.Range(0, 100) > MutationManager.MutationsTable.mutationChance) {
            return;
        }
        var options = GetMutationManagerForUnit(unit).GenerateOptions();

        await WriteLineAsync("");
        await WriteLineAsync("");
        await WriteLineAsync(unit.Name + " mutates.");
        await View.ShowMutationMenuRoutine(options);
        var selection = -1;
        while (selection < 0) {
            selection = await View.mutationSelector.SelectItemAsync(null, true);
        }

        var mutation = options[selection];
        mutation.Apply();
        await View.HideMutationMenuRoutine();
        await View.WriteLineRoutine(mutation.Message);
        await Global.Instance().Input.AwaitConfirm();
    }

    private async Task<bool> DoMeatAsync() {
        var roll = UnityEngine.Random.Range(0, 100);
        var offset = UnityEngine.Random.Range(0, Enemy.Size);
        Unit dropper = null;
        for (var i = 0; i < Enemy.Size; i += 1) {
            var index = (i + offset) % Enemy.Size;
            if (roll < Enemy[index].MeatDropChance) {
                dropper = Enemy[index];
                break;
            }
        }
        if (dropper == null) {
            return false;
        }

        await WriteLineAsync("");
        await WriteLineAsync("");
        await WriteLineAsync("Found meat of " + dropper.Name + ".");
        await View.ShowEatYesNoMenuRoutine();

        var command = await View.eatYesNoSelector.SelectCommandAsync();
        await View.HideEatYesNoRoutine();
        if (command != "EAT") {
            return false;
        }

        await View.ShowEaterMenuRoutine(dropper);
        var selection = await View.eaterSelector.SelectItemAsync();
        await View.HideEaterMenuRoutine();
        if (selection < 0) {
            return false;
        }
        
        var eater = Player[selection];
        if (eater == null) {
            return false;
        }

        CharaData newForm = MonsterFamily.GetTransformResult(eater, dropper);
        if (newForm != null) {
            await WriteLineAsync(eater.Name + " transformed into " + newForm.species + ".");
            MonsterFamily.TransformEater(eater, dropper);
        } else {
            await WriteLineAsync("Nothing happened.");
        }
        await Global.Instance().Input.AwaitConfirm();
        return true;
    }

    #endregion

    #region Util

    public async Task WriteLineAsync(string line = "", bool awaitConfirm = false) {
        await View.WriteLineRoutine(line);
        if (awaitConfirm) {
            await Global.Instance().Input.AwaitConfirm();
        }
    }

    #endregion
}
