[UnityEngine.CreateAssetMenu(fileName="EffectPassive", menuName="Data")]
public class EffectPassiveData : AbilEffectData {

    [Newtonsoft.Json.JsonConverter(typeof(StatModDeserializer))]
    public StatSet stats;
}
