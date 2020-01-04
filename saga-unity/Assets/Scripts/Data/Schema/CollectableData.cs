[UnityEngine.CreateAssetMenu(fileName="Collectable", menuName="Data/Rpg/")]
public class CollectableData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Display name - special symbols are allowed, 8 characters max maybe?")]
    public string displayName;
    
    [UnityEngine.Tooltip("Chest name - Specialty key for parts of items, what it\'s called coming out of the" +
        " chest, just leave blank")]
    public string chestName;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
