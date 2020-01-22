using Mgne1;

[System.Serializable]
public class MixEntryData {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    public CollectableData item;

    [UnityEngine.Tooltip("Quantity")]
    public int quantity;
}
