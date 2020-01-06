[UnityEngine.CreateAssetMenu(fileName="EncounterSet", menuName="Data/Rpg/")]
public class EncounterSetData : UnityEngine.ScriptableObject {

    [UnityEngine.Tooltip("Possible encounters in this set")]
    public EncounterSetMemberData encounters;

    [UnityEngine.Tooltip("Frequency - encounters will occur once per this many steps")]
    public int steps;
}
