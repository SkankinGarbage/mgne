using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BattleView : FullScreenMenuView {

    [SerializeField] public GenericSelector fightRunMenu = null;
    [SerializeField] public CombatItemList inventory = null;
    [SerializeField] public UnitList allyList = null;
    [SerializeField] public GenericSelector enemySelector = null;
    [SerializeField] public BattleBox battlebox = null;
    [SerializeField] public ListSelector mutationSelector = null;
    [SerializeField] private UnitList unitList = null;
    [SerializeField] private ListView dollList = null;
    [SerializeField] private ListView battlerList = null;
    [SerializeField] private ListView monsterNameList = null;
    [SerializeField] private UnitCellView unitCell = null;
    [SerializeField] private ListView mutationView = null;

    public Battle Battle { get; private set; }

    public static BattleView Show(PartyData enemyParty) {
        var battle = new Battle(enemyParty);
        var menu = Instantiate<BattleView>("Prefabs/UI/Battle/BattleView");
        menu.Populate(battle);

        Global.Instance().Input.PushListener(menu.ToString(), (cmd, x) => true);

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

        unitList.Populate();
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
        yield return WriteLineRoutine(actor.Name + " does nothing.");
    }

    public IEnumerator PlayBackDamageRoutine(Unit target, int damage) {
        yield return WriteLineRoutine(BattleBox.Tab + target.Name + " takes " + damage + " damage.");
        // TODO
        yield return null; 
    }

    public IEnumerator ShowMutationMenuRoutine(List<Mutation> mutations) {
        for (var i = 0; i < 6; i += 1) yield return WriteLineRoutine("");
        mutationSelector.gameObject.SetActive(true);
        mutationView.Populate(mutations, (obj, mutation) => {
            obj.GetComponent<Text>().text = mutation.Description;
        });
        yield return mutationSelector.GetComponent<ExpanderComponent>().ShowRoutine();
    }

    public IEnumerator HideMutationMenuRoutine() {
        yield return mutationSelector.GetComponent<ExpanderComponent>().HideRoutine();
    }

    public override IEnumerator CloseRoutine() {
        Global.Instance().Input.RemoveListener(ToString());
        return base.CloseRoutine();
    }
}
