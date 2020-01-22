using Mgne1;

[UnityEngine.CreateAssetMenu(fileName="ItemMix", menuName="Data/Rpg")]
public class ItemMixData : MainSchema {

    [UnityEngine.Tooltip("Ingredients")]
    public MixEntryData[] ingredients;

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("The resulting combat item")]
    public CombatItemData result;
}
