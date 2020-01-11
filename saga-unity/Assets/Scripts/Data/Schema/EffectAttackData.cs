[UnityEngine.CreateAssetMenu(fileName="EffectAttack", menuName="Data")]
public class EffectAttackData : EffectCombatData {

    [UnityEngine.Tooltip("Base power - power of the attack before multipliers")]
    public int power = 0;

    [UnityEngine.Tooltip("Attack stat - this stat is quartered and then multiplied by power")]
    public StatTag? attackStat = StatTag.STR;

    [UnityEngine.Tooltip("Defend stat - this stat is subtracted from incoming damage")]
    public StatTag? defendStat = StatTag.DEF;

    [UnityEngine.Tooltip("Miss type - generally only melee attacks should miss")]
    public MissType miss = MissType.CAN_MISS;
}
