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

    public Battle(PartyData enemy) {
        Player = Global.Instance().Party;
        Enemy = new Party(enemy);
    }

    public IEnumerator BattleRoutine(BattleView view) {
        View = view;
        yield return CoUtils.TaskRoutine(BattleAsync());
    }

    private async Task BattleAsync() {
        await Task.Delay(5000);
    }
}
