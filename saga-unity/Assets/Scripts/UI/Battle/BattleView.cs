using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class BattleView : FullScreenMenuView {
    
    [SerializeField] public GenericSelector fightRunMenu = null;
    [SerializeField] public CombatItemList inventory = null;
    [SerializeField] public UnitList allyList = null;
    [SerializeField] public GenericSelector enemySelector = null;
    [SerializeField] public BattleBox battlebox = null;
    [SerializeField] public ListSelector mutationSelector = null;
    [SerializeField] public ListSelector eatYesNoSelector = null;
    [SerializeField] public DynamicListSelector eaterSelector = null;
    [SerializeField] public AnimFrameSpritePool framePool = null;
    [SerializeField] public RetryView retry = null;
    [SerializeField] private UnitList unitList = null;
    [SerializeField] private ListView dollList = null;
    [SerializeField] private ListView battlerList = null;
    [SerializeField] private ListView monsterNameList = null;
    [SerializeField] private UnitCellView unitCell = null;
    [SerializeField] private ListView mutationView = null;
    [SerializeField] private ListView eaterView = null;
    [SerializeField] private DollArea dollArea = null;

    public Battle Battle { get; private set; }

    public static async Task<BattleView> ShowAsync(Party enemy) {
        var battle = new Battle(enemy);
        await SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Additive);
        var menu = FindObjectOfType<BattleView>();
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
            var posRatio = (float)(battle.Enemy.Groups.IndexOf(group) + 1) / (battle.Enemy.Groups.Count + 1);
            obj.GetComponent<EnemyDoll>().Populate(group, dollArea, posRatio);
        });
        monsterNameList.Populate(battle.Enemy.Groups, (obj, group) => {
            obj.GetComponent<EnemyNameCellView>().Populate(group);
        });
    }

    public void UpdateForMemberChange() {
        Populate(Battle);
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

    public List<AnimFrameSpriteComponent> ShowBattleAnimationFrame(BattleStepData data, List<Unit> targets) {
        var groupIndices = new HashSet<int>();
        foreach (var target in targets) {
            groupIndices.Add(Battle.Enemy.GroupIndexForUnit(target));
        }

        var frames = new List<AnimFrameSpriteComponent>();
        foreach (var index in groupIndices) {
            var targetCell = battlerList.GetCell(index);
            var frame = framePool.GetFrame(data, dollArea.OffsetForEnemy(Battle.Enemy.Groups[index]));
            frames.Add(frame);
        }
        return frames;
    }

    public void HideBattleAnimationFrames(List<AnimFrameSpriteComponent> frames) {
        foreach (var frame in frames) {
            framePool.DisposeFrame(frame);
        }
    }

    public static IEnumerator SpawnBattleRoutine(PartyData enemyData, string bgmTag = null) {
        return SpawnBattleRoutine(new Party(enemyData), bgmTag);
    }
    public static IEnumerator SpawnBattleRoutine(Party enemy, string bgmTag = null) {
        if (bgmTag == null) {
            bgmTag = Global.Instance().Data.BattleBGMKey;
        }
        Global.Instance().Audio.PlayBGM(bgmTag);
        var task = ShowAsync(enemy);
        yield return CoUtils.TaskRoutine(task);
        var menu = task.Result;
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
        yield return CoUtils.RunParallel(new IEnumerator[] {
            WriteLineRoutine(BattleBox.Tab + target.Name + " takes " + damage + " damage."),
            AnimateDamageRoutine(target, damage),
        }, this);
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

    public IEnumerator ShowEatYesNoMenuRoutine() {
        for (var i = 0; i < 5; i += 1) yield return WriteLineRoutine("");
        yield return eatYesNoSelector.GetComponent<ExpanderComponent>().ShowRoutine();
    }

    public IEnumerator HideEatYesNoRoutine() {
        yield return eatYesNoSelector.GetComponent<ExpanderComponent>().HideRoutine();
    }

    public IEnumerator ShowEaterMenuRoutine(Unit dropper) {
        eaterSelector.gameObject.SetActive(true);
        eaterView.Populate(Global.Instance().Party, (obj, unit) => {
            obj.GetComponent<EaterCell>().Populate(unit, MonsterFamily.GetTransformResult(unit, dropper));
        });
        eaterSelector.gameObject.SetActive(true);
        yield return null;
    }

    public IEnumerator HideEaterMenuRoutine() {
        eaterSelector.gameObject.SetActive(false);
        yield return null;
    }

    public IEnumerator AnimateDamageRoutine(Unit target, int damage) {
        foreach (Transform cell in dollList.GetCells()) {
            var battler = cell.GetComponent<BattlerDoll>();
            if (battler.Unit == target) {
                yield return battler.TakeDamageRoutine(damage);
                yield break;
            }
        }
    }

    public override IEnumerator CloseRoutine() {
        Global.Instance().Input.RemoveListener(ToString());
        yield return CoUtils.TaskRoutine(CloseAsync());
    }

    public async Task CloseAsync() {
        await SceneManager.UnloadSceneAsync("Battle");
    }
}
