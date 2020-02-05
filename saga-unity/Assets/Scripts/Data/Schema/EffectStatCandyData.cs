[UnityEngine.CreateAssetMenu(fileName="EffectStatCandy", menuName="Data")]
public class EffectStatCandyData : AbilEffectData {

    [UnityEngine.Tooltip("Stat")]
    public StatTag stat;

    [UnityEngine.Tooltip("Min gain")]
    public int minGain;

    [UnityEngine.Tooltip("Max gain")]
    public int maxGain;

    [UnityEngine.Tooltip("Restricted races - only these races allowed to consume")]
    public Race[] restrictRace;

    [UnityEngine.Tooltip("Maximum effective value - characters with a stat value above this will receive no" +
" effect from this item, or 0 for no maximum")]
    public int maxValue;
}
