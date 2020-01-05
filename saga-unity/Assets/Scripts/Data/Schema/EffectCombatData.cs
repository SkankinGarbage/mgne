[UnityEngine.CreateAssetMenu(fileName="EffectCombat", menuName="Data/")]
public class EffectCombatData : EffectEnemyTargetData {

    [UnityEngine.Tooltip("Damage type")]
    public DamageType damType = DamageType.PHYSICAL;

    [UnityEngine.Tooltip("Other flags")]
    public OffenseFlag sideEffects;

    [UnityEngine.Tooltip("Slayer families - monsters in these families will be weak")]
    public MonsterFamilyData[] slayerFamiles;

    [UnityEngine.Tooltip("Riders - an invisible defensive ability that also applies")]
    public EffectDefendData[] riders;




}
