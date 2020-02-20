using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SaveInfoData {

    public string LeaderName { get; private set; }
    public string Location { get; private set; }
    public long Timestamp { get; private set; }
    public List<string> FieldSpriteTags { get; private set; }

    public SaveInfoData(GameData data) {
        LeaderName = data.Party.Leader.Name;
        Location = data.LocationName;
        Timestamp = data.SavedAt;
        FieldSpriteTags = new List<string>(data.Party.Select(unit => unit.FieldSpriteTag));
    }
}
