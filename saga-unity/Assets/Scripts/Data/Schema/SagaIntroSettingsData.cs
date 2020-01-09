[UnityEngine.CreateAssetMenu(fileName="SagaIntroSettings", menuName="Data/Settings/")]
public class SagaIntroSettingsData : MainSchema {

    [UnityEngine.Tooltip("Intro text - sloooowly scrolls, make sure to use \\n etc")]
    public string introText;

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Leader recruiting selection (x1)")]
    public RecruitSelectionData recruitLeader;

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Member recruiting selection (x3)")]
    public RecruitSelectionData recruitMember;

    [UnityEngine.Tooltip("Race help text - use \\n")]
    public string raceText;
}
