using Mgne1;

[System.Serializable]
public class EncounterMemberData {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Enemy")]
    public CharaData enemy;

    [UnityEngine.Tooltip("Possible amount - max-min syntax, eg 0-2 or 1-4")]
    public string amount;
}
