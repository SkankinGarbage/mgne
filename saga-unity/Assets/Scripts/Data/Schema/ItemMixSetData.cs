using Mgne1;

[UnityEngine.CreateAssetMenu(fileName="ItemMixSet", menuName="Data/Rpg")]
public class ItemMixSetData : MainSchema {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("The available combinations")]
    public ItemMixData[] combinations;
}
