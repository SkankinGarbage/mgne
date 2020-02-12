using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

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
        while (!IsDone) {
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
    }

    private async Task FightAsync() {
        await Task.Delay(100);
    }

    private async Task EndCombatAsync() {
        IsDone = true;
        await View.CloseRoutine();
        // TODO: handle death + retry
    }

    #endregion

    #region Model

    /// <returns>True if got away successfully</returns>
    private bool TryEscapeChance() {
        return true; // lol
    }

    #endregion
}
