[UnityEngine.CreateAssetMenu(fileName="EffectRevive", menuName="Data/")]
public class EffectReviveData : EffectAllyTargetData {

    [UnityEngine.Tooltip("Heal base - after resurrection, targets starts with this much health")]
    public int @base = 0;

    [UnityEngine.Tooltip("Heal power - multiplier for the heal (added to base)")]
    public int power = 0;

    [UnityEngine.Tooltip("Power stat - this stat is quartered and multiplied by power")]
    public StatTag powerStat = StatTag.None;

    
    
}
