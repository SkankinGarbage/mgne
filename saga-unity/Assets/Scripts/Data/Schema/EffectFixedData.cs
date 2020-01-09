[UnityEngine.CreateAssetMenu(fileName="EffectFixed", menuName="Data/")]
public class EffectFixedData : EffectCombatData {

    [UnityEngine.Tooltip("Base damage")]
    public int @base = 0;

    [UnityEngine.Tooltip("Damage range - will deal up from base to base + range damage")]
    public int range = 0;

    [UnityEngine.Tooltip("Defense stat - deducted from damage")]
    public StatTag? defenseStat = StatTag.DEF;

    [UnityEngine.Tooltip("Item accuracy - base chance to hit, usually 0-100")]
    public int accuracy = 100;

    [UnityEngine.Tooltip("Accuracy stat - chance to hit increased by this")]
    public StatTag? accStat = StatTag.AGI;

    [UnityEngine.Tooltip("Dodge stat - chance to hit reduced by this")]
    public StatTag? dodgeStat = StatTag.AGI;
}
