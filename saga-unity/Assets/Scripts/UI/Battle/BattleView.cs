using UnityEngine;
using System.Collections;

public class BattleView : FullScreenMenuView {

    [SerializeField] public GenericSelector fightRunMenu = null;
    [SerializeField] public CombatItemList inventory = null;
    [SerializeField] public UnitList allyList = null;
    [SerializeField] public GenericSelector enemySelector = null;
    [SerializeField] public BattleBox battlebox = null;
    [SerializeField] private UnitList unitList = null;
    [SerializeField] private ListView dollList = null;
    [SerializeField] private ListView battlerList = null;
    [SerializeField] private ListView monsterNameList = null;
    [SerializeField] private UnitCellView unitCell = null;

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

    public void PopulateForUnitIntentSelect(Intent intent) {
        battlebox.gameObject.SetActive(false);
        unitCell.gameObject.SetActive(true);
        inventory.gameObject.SetActive(true);
        allyList.gameObject.SetActive(false);

        inventory.Populate(intent.Actor.Equipment);
        unitCell.Populate(intent.Actor);
    }

    public void PopulateForFightRun() {
        battlebox.gameObject.SetActive(false);
        unitCell.gameObject.SetActive(false);
        inventory.gameObject.SetActive(false);
        allyList.gameObject.SetActive(false);
    }

    public void PopulateForAllySelection() {
        battlebox.gameObject.SetActive(false);
        inventory.gameObject.SetActive(true);
        allyList.gameObject.SetActive(true);

        allyList.Populate();
    }

    public static IEnumerator SpawnBattleRoutine(PartyData data, string bgmTag = null) {
        if (bgmTag == null) {
            bgmTag = Global.Instance().Data.CurrentBGMKey;
        }
        Global.Instance().Audio.PlayBGM(bgmTag);
        var menu = Show(data);
        yield return menu.Battle.BattleRoutine(menu);
    }

    public IEnumerator WriteLineRoutine(string line) {
        if (!battlebox.isActiveAndEnabled) {
            battlebox.gameObject.SetActive(true);
            yield return battlebox.ShowRoutine();
        }
        yield return battlebox.WriteLineRoutine(line);
    }

    public IEnumerator HideBattleboxRoutine() {
        if (battlebox.isActiveAndEnabled) {
            yield return battlebox.HideRoutine();
        }
    }

    public IEnumerator PrintDoesNothingRoutine(Unit actor) {
        yield return WriteLineRoutine(actor + " does nothing.");
        yield return WriteLineRoutine("");
    }
}
