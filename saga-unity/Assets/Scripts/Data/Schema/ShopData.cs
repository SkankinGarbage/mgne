[UnityEngine.CreateAssetMenu(fileName="Shop", menuName="Data/Rpg/")]
public class ShopData : MainSchema {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Available items")]
    public CombatItemData[] items;
}
