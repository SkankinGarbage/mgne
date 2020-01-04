[UnityEngine.CreateAssetMenu(fileName="MeatShop", menuName="Data/Rpg/")]
public class MeatShopData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Shop inventory")]
    public MeatShopEntryData entries;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
