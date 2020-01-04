[UnityEngine.CreateAssetMenu(fileName="RecruitSelection", menuName="Data/Rpg/")]
public class RecruitSelectionData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("The title of the recruit text")]
    public string title;
    
    [UnityEngine.Tooltip("Recruitable characters")]
    public CharaData options;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
