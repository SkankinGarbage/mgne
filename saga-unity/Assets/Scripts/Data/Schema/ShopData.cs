[UnityEngine.CreateAssetMenu(fileName="Shop", menuName="Data/Rpg/")]
public class ShopData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Available items")]
    public CombatItemData items;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
