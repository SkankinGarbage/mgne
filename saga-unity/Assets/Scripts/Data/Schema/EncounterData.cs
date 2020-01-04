[UnityEngine.CreateAssetMenu(fileName="Encounter", menuName="Data/Rpg/")]
public class EncounterData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Members in this encounter")]
    public EncounterMemberData members;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
