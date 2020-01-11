[UnityEngine.CreateAssetMenu(fileName="EncounterSet", menuName="Data/Rpg")]
public class EncounterSetData : MainSchema {
    
    [UnityEngine.Tooltip("Possible encounters in this set")]
    public EncounterSetMemberData[] encounters;

    [UnityEngine.Tooltip("Frequency - encounters will occur once per this many steps")]
    public int steps;
}
