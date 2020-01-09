[System.Serializable]
public class MeatShopEntryData {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    public CharaData monster;

    [UnityEngine.Tooltip("Price")]
    public int price;
}
