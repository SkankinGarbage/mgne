[UnityEngine.CreateAssetMenu(fileName="EncounterMember", menuName="Data/")]
public class EncounterMemberData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Enemy")]
    public CharaData enemy;
    
    [UnityEngine.Tooltip("Possible amount - max-min syntax, eg 0-2 or 1-4")]
    public string amount;
}
