[UnityEngine.CreateAssetMenu(fileName="Chara", menuName="Data/Rpg/")]
public class CharaData : MainSchema {

    [UnityEngine.Tooltip("Gender")]
    public Gender gender = Gender.NONE;

    [UnityEngine.Tooltip("Race")]
    public Race race;

    [UnityEngine.Tooltip("Family - used for transformations, only monsters should have this")]
    public MonsterFamilyData family;

    [UnityEngine.Tooltip("Species - the name of the specific monster subtype, eg GOBLIN, only monsters shou" +
"ld have this")]
    public string species;

    [UnityEngine.Tooltip("Appearance")]
    // TODO: mgne
    public string appearance;

    [UnityEngine.Tooltip("In-battle portrait")]
    public string portrait = "None";

    [UnityEngine.Tooltip("Meat eat level - the power of the meat this character drops, compared to the tran" +
"sform level of other monsters, monster only")]
    public int meatEatLevel;

    [UnityEngine.Tooltip("Meat transform level - the power of meat needed to transform into this character," +
" compared to the eat level of others")]
    public int meatTargetLevel;

    [UnityEngine.Tooltip("Percent out of 100 chance of dropping meat - 0 to use default")]
    public int meatDropChance = 0;

    [UnityEngine.Tooltip("GP - dropped when this character is defeated by the player")]
    public int gp = 0;

    [UnityEngine.Tooltip("Loot - dropped when this monster is defeated")]
    public CombatItemData loot;

    [UnityEngine.Tooltip("Death animation - played on death, null for default")]
    public BattleAnimData deathAnim;

    [UnityEngine.Tooltip("Percent out of 100 chance of dropping loot - 0 to use default")]
    public int lootDropChance = 0;

    [UnityEngine.Tooltip("Equipped items/abilities")]
    public CombatItemData equipped;

    public StatSetData stats;
}
