[UnityEngine.CreateAssetMenu(fileName="RecruitSelection", menuName="Data/Rpg/")]
public class RecruitSelectionData : MainSchema {

    [UnityEngine.Tooltip("The title of the recruit text")]
    public string title;

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Recruitable characters")]
    public CharaData[] options;
}
