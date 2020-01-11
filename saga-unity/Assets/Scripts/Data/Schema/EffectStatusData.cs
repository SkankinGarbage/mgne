[UnityEngine.CreateAssetMenu(fileName="EffectStatus", menuName="Data")]
public class EffectStatusData : EffectEnemyTargetData {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Status to inflict")]
    public StatusData status;

    [UnityEngine.Tooltip("Accuracy - base chance to hit, from 0 to 100 usually")]
    public int hit = 80;

    [UnityEngine.Tooltip("Accuracy stat - added to chance to hit")]
    public StatTag? accStat = StatTag.MANA;

    [UnityEngine.Tooltip("Evasion stat - subtracted from chance to hit")]
    public StatTag? evadeStat = StatTag.MANA;
}
