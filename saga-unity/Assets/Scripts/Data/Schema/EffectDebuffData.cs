[UnityEngine.CreateAssetMenu(fileName="EffectDebuff", menuName="Data/")]
public class EffectDebuffData : EffectEnemyTargetData {

    [UnityEngine.Tooltip("Base power - power of the debuff before multipliers")]
    public int power = 0;

    [UnityEngine.Tooltip("Affected stat - the stat to drain from the enemy")]
    public StatTag drainStat = StatTag.STR;

    [UnityEngine.Tooltip("Attack stat - this stat is halved and then multiplied by power")]
    public StatTag attackStat = StatTag.MANA;

    [UnityEngine.Tooltip("Defend stat - this stat is subtracted from incoming stat drain")]
    public StatTag defendStat = StatTag.MANA;

    
    
}
