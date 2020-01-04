[UnityEngine.CreateAssetMenu(fileName="MonsterSettings", menuName="Data/Settings/")]
public class MonsterSettingsData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Meat abundancy - chance that a group drops meat from 0 to 100")]
    public int meatChance;
    
    [UnityEngine.Tooltip("Loot abundancy - chance that a group drops a loot item (if available)")]
    public int lootChance;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
