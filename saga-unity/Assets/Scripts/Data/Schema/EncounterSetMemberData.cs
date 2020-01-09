[System.Serializable]
public class EncounterSetMemberData {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("The encounter in this set")]
    public EncounterData encounter;

    [UnityEngine.Tooltip("The weight this encounter will appear vs others in the set")]
    public int weight;
}
