[UnityEngine.CreateAssetMenu(fileName="Party", menuName="Data/Rpg/")]
public class PartyData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Party members")]
    public PartyEntryData members;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
