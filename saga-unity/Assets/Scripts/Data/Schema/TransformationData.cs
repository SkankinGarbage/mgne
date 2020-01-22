using Mgne1;

[System.Serializable]
public class TransformationData {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Meat group eaten")]
    public MeatGroupData eat;

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Resulting family")]
    public MonsterFamilyData result;
}
