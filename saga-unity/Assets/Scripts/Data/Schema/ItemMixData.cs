[UnityEngine.CreateAssetMenu(fileName="ItemMix", menuName="Data/Rpg/")]
public class ItemMixData : UnityEngine.ScriptableObject {

    [UnityEngine.Tooltip("Ingredients")]
    public MixEntryData ingredients;

    [UnityEngine.Tooltip("The resulting combat item")]
    public CombatItemData result;

    
    
}
