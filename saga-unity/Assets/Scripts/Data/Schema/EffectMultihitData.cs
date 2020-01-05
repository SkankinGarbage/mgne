[UnityEngine.CreateAssetMenu(fileName="EffectMultihit", menuName="Data/")]
public class EffectMultihitData : EffectCombatData {

    [UnityEngine.Tooltip("Base power - base multiplier of each hit")]
    public int power;

    [UnityEngine.Tooltip("Hits order - the attack will hit this many times with each use, comma separated, " +
"like \"3,2,2,1,1\" and then cycle")]
    public string hits;

    [UnityEngine.Tooltip("Attack stat - this stat is quartered and then multiplied by power")]
    public StatTag attackStat = StatTag.STR;

    [UnityEngine.Tooltip("Defend stat - this stat is subtracted from incoming damage")]
    public StatTag defendStat = StatTag.DEF;




}
