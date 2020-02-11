using UnityEngine;
using System.Collections;

public class BattleView : FullScreenMenuView {

    [SerializeField] private BattleBox battlebox = null;
    [SerializeField] private UnitList unitList = null;
    [SerializeField] private GenericSelector fightRunMenu = null;
    [SerializeField] private ListView dollList = null;
    [SerializeField] private ListView battlerList = null;
    [SerializeField] private ListView monsterNameList = null;

    public Battle Battle { get; private set; }

    public static BattleView Show(PartyData enemyParty) {
        var battle = new Battle(enemyParty);
        var menu = Instantiate<BattleView>("Prefabs/UI/Battle/BattleView");
        menu.Populate(battle);
        return menu;
    }

    public void Populate(Battle battle) {
        Battle = battle;
        unitList.Populate();
        dollList.Populate(battle.Player, (obj, unit) => {
            obj.GetComponent<BattlerDoll>().Populate(unit);
        });
        battlerList.Populate(battle.Enemy.Groups, (obj, group) => {
            obj.GetComponent<EnemyDoll>().Populate(group);
        });
        monsterNameList.Populate(battle.Enemy.Groups, (obj, group) => {
            obj.GetComponent<EnemyNameCellView>().Populate(group);
        });
    }

    public static IEnumerator SpawnBattleRoutine(PartyData data, string bgmTag = null) {
        if (bgmTag == null) {
            bgmTag = Global.Instance().Data.CurrentBGMKey;
        }
        Global.Instance().Audio.PlayBGM(bgmTag);
        var menu = Show(data);
        yield return menu.Battle.BattleRoutine(menu);
    }
}
