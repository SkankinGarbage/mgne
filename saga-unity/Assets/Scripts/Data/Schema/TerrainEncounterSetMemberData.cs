using Mgne1;

[System.Serializable]
public class TerrainEncounterSetMemberData {
    
    [UnityEngine.Tooltip("Terrain - in format [tilesetname]/[id], find both properties in the Tiled editor")]
    public string terrain;

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    public EncounterSetData encounters;
}
