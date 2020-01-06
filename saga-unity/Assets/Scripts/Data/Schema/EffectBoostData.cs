[UnityEngine.CreateAssetMenu(fileName="EffectBoost", menuName="Data/")]
public class EffectBoostData : EffectAllyTargetData {

    [UnityEngine.Tooltip("Stat to boost")]
    public StatTag stat = StatTag.STR;

    [UnityEngine.Tooltip("Base power - power of the boost before modifiers")]
    public int power = 0;

    [UnityEngine.Tooltip("Power stat - this stat is added to base power")]
    public StatTag powerStat = StatTag.None;

    [UnityEngine.Tooltip("Cap - boosted stat will never exceed this value, 0 for unlimited")]
    public int cap;
}
