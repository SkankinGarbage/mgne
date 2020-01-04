[UnityEngine.CreateAssetMenu(fileName="TerrainEncounterSetMember", menuName="Data/")]
public class TerrainEncounterSetMemberData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Terrain - in format [tilesetname]/[id], find both properties in the Tiled editor")]
    public string terrain;
    
    public EncounterSetData encounters;
}
