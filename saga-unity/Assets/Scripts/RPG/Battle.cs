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

    public bool IsDone { get; private set; }
    public bool IsVictory => !Enemy.IsAnyAlive;
    public bool IsDefeat => !Player.IsAnyAlive;

    private Intent[] playerIntents;
    private List<Intent> allIntents;
    private Dictionary<Unit, List<EffectDefend>> defenses;
    private List<TempStats> boosts;

    public Battle(PartyData enemy) {
        Player = Global.Instance().Party;
        Enemy = new Party(enemy);

        boosts = new List<TempStats>();
        defenses = new Dictionary<Unit, List<EffectDefend>>();
    }

    #region Model

    public List<EffectDefend> GetDefensesForUnit(Unit unit) {
        if (defenses.ContainsKey(unit)) {
            return defenses[unit];
        } else {
            return new List<EffectDefend>();
        }
    }

    public void AddBoost(Unit target, StatSet boost) {
        boosts.Add(new TempStats(target, boost));
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
        if (!silent) await WriteLineAsync(BattleBox.Tab + target.Name + " is defeated.");
    }

    /// <returns>True if got away successfully</returns>
    private bool TryEscapeChance() {
        return true; // lol
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
            var fightRunCommand = await View.fightRunMenu.SelectCommandAsync();
            switch (fightRunCommand) {
                case "FIGHT":
                    await FightAsync();
                    break;
                case "RUN":
                    await RunAsync();
                    break;
            }
        }
        await EndCombatAsync();
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
                allIntents.Add(ConstructIntentForEnemy(enemy));
            }
        }
        allIntents.Sort(new Comparison<Intent>((a, b) => a.Priority - b.Priority));

        await ResolveIntentsAsync();
        await Global.Instance().Input.AwaitConfirm();

        return true;
    }

    private async Task EndCombatAsync() {
        foreach (var boost in boosts) {
            boost.Decombine();
        }
        boosts.Clear();

        await View.CloseRoutine();
        // TODO: handle death + retry
    }

    /// <returns>True if succeeded, false if canceled</returns>
    private async Task<bool> ConstructIntentForPlayerAsync(Intent intent) {
        View.PopulateForUnitIntentSelect(intent);
        var selector = View.inventory.Selector;
        selector.Selection = intent.FindIndexForItem();
        do {
            var slot = await selector.SelectItemAsync(null, true);
            selector.ClearSelection();
            if (slot == -1) {
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

    private Intent ConstructIntentForEnemy(Unit enemy) {
        return enemy.AI.ConstructIntent(this);
    }

    private int AdvanceUnitIndex(int index, int delta) {
        do {
            index += delta;
        } while (index < Player.Size && index >= 0 && !Player[index].CanAct);
        return index;
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
