using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

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

    private Intent[] intents;

    public Battle(PartyData enemy) {
        Player = Global.Instance().Party;
        Enemy = new Party(enemy);
    }

    public async Task WriteLineAsync(string line = "", bool awaitConfirm = false) {
        await View.WriteLineRoutine(line);
        if (awaitConfirm) {
            await Global.Instance().Input.AwaitConfirm();
        }
    }

    public IEnumerator BattleRoutine(BattleView view) {
        View = view;
        yield return CoUtils.TaskRoutine(BattleAsync());
    }

    #region Control

    private async Task BattleAsync() {
        Global.Instance().Input.PushListener(ToString(), (cmd, ev) => { return true; });
        while (!IsDone) {
            InitializeIntents();
            View.PopulateForFightRun();
            var fightRunCommand = await View.fightRunMenu.SelectCommandAsync();
            switch (fightRunCommand) {
                case "FIGHT":
                    await FightAsync();
                    break;
                case "RUN":
                    if (TryEscapeChance()) {
                        await WriteLineAsync();
                        await WriteLineAsync("Escaped!", true);
                        await EndCombatAsync();
                    }
                    break;
            }
        }
        Global.Instance().Input.RemoveListener(ToString());
    }

    /// <returns>True if succeeded, false if canceled</returns>
    private async Task<bool> FightAsync() {
        int unitIndex = 0;
        while (unitIndex < Player.Size && unitIndex >= 0) {
            var succeeded = await ConstructIntentForPlayerAsync(intents[unitIndex]);
            unitIndex += succeeded ? 1 : -1;
        }
        return unitIndex > 0;
    }

    private async Task EndCombatAsync() {
        IsDone = true;
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

    #endregion

    #region Model

    /// <returns>True if got away successfully</returns>
    private bool TryEscapeChance() {
        return true; // lol
    }

    private void InitializeIntents() {
        intents = new Intent[Player.Size];
        for (var i = 0; i < Player.Size; i += 1) {
            intents[i] = new Intent(Player[i], this);
        }
    }

    #endregion
}
