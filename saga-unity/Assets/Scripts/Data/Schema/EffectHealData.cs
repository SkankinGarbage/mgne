[UnityEngine.CreateAssetMenu(fileName="EffectHeal", menuName="Data/")]
public class EffectHealData : EffectAllyTargetData {

    [UnityEngine.Tooltip("Heal base - base power for the heal, added to final value")]
    public int @base = 0;

    [UnityEngine.Tooltip("Heal power - multiplier for the heal")]
    public int power = 0;

    [UnityEngine.Tooltip("Power stat - this stat is quartered and multiplied by power")]
    public StatTag powerStat = StatTag.None;

    [UnityEngine.Tooltip("Heal status - cures these ailments")]
    public StatusData[] heals;

    [UnityEngine.Tooltip("Use restore - does this heal restore uses? (for tent basically)")]
    public UseRestoreType useRestore = UseRestoreType.NO_USE_RESTORE;
}
