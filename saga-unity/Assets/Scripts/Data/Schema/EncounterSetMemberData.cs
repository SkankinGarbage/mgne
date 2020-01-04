[UnityEngine.CreateAssetMenu(fileName="EncounterSetMember", menuName="Data/")]
public class EncounterSetMemberData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("The encounter in this set")]
    public EncounterData encounter;
    
    [UnityEngine.Tooltip("The weight this encounter will appear vs others in the set")]
    public int weight;
}
