using System.Collections.Generic;

public class TerrainEncounterSet : BaseEncounterSet {

    private TerrainEncounterSetData data;

    public TerrainEncounterSet(TerrainEncounterSetData data) {
        this.data = data;
    }

    protected override int AverageStepsToEncounter {
        get {
            var set = GetCurrentSet();
            if (set == null) {
                return 0;
            } else {
                return set.encounters.steps;
            }
        }
    }

    protected override List<EncounterSetMemberData> GetEncounters() {
        return new List<EncounterSetMemberData>(GetCurrentSet().encounters.encounters);
    }

    protected TerrainEncounterSetMemberData GetCurrentSet() {
        var avatar = Global.Instance().Maps.Avatar;
        var tile = avatar.Map.TileAt(avatar.Map.layers[0], avatar.Parent.Position.x, avatar.Parent.Position.y);
        if (tile == null) {
            return null;
        }
        foreach (var member in data.members) {
            var terrainString = member.terrain;
            var idString = terrainString.Substring(terrainString.IndexOf('/') + 1);
            var id = int.Parse(idString);
            if (tile.TerrainId == id) {
                return member;
            }
        }
        return null;
    }
}
