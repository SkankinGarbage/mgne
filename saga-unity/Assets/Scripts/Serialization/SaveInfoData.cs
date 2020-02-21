using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

[JsonObject(MemberSerialization.OptIn)]
public class SaveInfoData {

    [JsonProperty] public string LeaderName { get; private set; }
    [JsonProperty] public string Location { get; private set; }
    [JsonProperty] public long Timestamp { get; private set; }
    [JsonProperty] public List<string> FieldSpriteTags { get; private set; }

    public SaveInfoData() {
        // serialized
    }

    public SaveInfoData(GameData data) {
        LeaderName = data.Party.Leader.Name;
        Location = data.LocationName;
        Timestamp = data.SavedAt;
        FieldSpriteTags = new List<string>(data.Party.Select(unit => unit.FieldSpriteTag));
    }
}
