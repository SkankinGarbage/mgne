[UnityEngine.CreateAssetMenu(fileName="Status", menuName="Data/Rpg/")]
public class StatusData : MainSchema {

    [UnityEngine.Tooltip("Full name - used in extended menus, eg BLIND")]
    public string fullName;

    [UnityEngine.Tooltip("Short name - used in small menus, eg BLND")]
    public string tag;

    [UnityEngine.Tooltip("Inflict message - printed when inflicted, eg \" is blind\"")]
    public string inflictString;

    [UnityEngine.Tooltip("Heal message - printed when healed, eg \" regains sight\"")]
    public string healString;

    [UnityEngine.Tooltip("Priority - effect with higher priority overwrite other effects")]
    public int priority = 50;

    [UnityEngine.Tooltip("Resist flag - characters with this flag are immune")]
    public StatTag resistFlag;

    [UnityEngine.Tooltip("Prevent action chance - chance from 0 to 100 chara will be unable to act in battl" +
"e")]
    public int preventChance = 0;

    [UnityEngine.Tooltip("Random action chance - chance from 0 to 100 chara will pick random move and targe" +
"t")]
    public int randomChance = 0;

    [UnityEngine.Tooltip("Recover chance - chance from 0 to 100 chara will randomly heal this status in bat" +
"tle")]
    public int recoverChance = 0;

    [UnityEngine.Tooltip("Lethality type - the game could treat this status the same as being dead")]
    public LethalityType lethality = LethalityType.NON_DEADLY;

    [UnityEngine.Tooltip("Recovery type")]
    public RecoverType recover;

    [UnityEngine.Tooltip("Reduced stats - in-battle values is reduced by 50% for these stats in battle")]
    public StatTag[] halvedStats;

    [UnityEngine.Tooltip("Damage over time - will be inflicted to the afflicted each turn, zero for none")]
    public int dot;

    [UnityEngine.Tooltip("Damage over time stat - quartered and multiplied by DoT power")]
    public StatTag? dotStat;
}
