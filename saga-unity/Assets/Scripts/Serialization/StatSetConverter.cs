using Newtonsoft.Json;
using System;

public class StatSetConverter : JsonConverter<StatSet> {

    public override StatSet ReadJson(JsonReader reader, Type objectType, StatSet existingValue, bool hasExistingValue, JsonSerializer serializer) {
        var data = serializer.Deserialize<SerializedStatSet>(reader);
        StatSet set = new StatSet(data);
        set.OnBeforeSerialize();
        return set;
    }

    public override void WriteJson(JsonWriter writer, StatSet value, JsonSerializer serializer) {
        var toSerialize = new SerializedStatSet(value);
        serializer.Serialize(writer, toSerialize);
    }
}
