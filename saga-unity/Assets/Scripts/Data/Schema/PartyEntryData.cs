[System.Serializable]
public class PartyEntryData {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Monster")]
    public CharaData monster;

    [UnityEngine.Tooltip("Count")]
    public int count = 1;
}
