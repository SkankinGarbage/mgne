using Newtonsoft.Json;
using System;

public class StatSetConverter : JsonConverter {

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        throw new NotImplementedException("WriteJson");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        SerializedStatSet data = serializer.Deserialize<SerializedStatSet>(reader);
        StatSet set = new StatSet(data);
        set.OnBeforeSerialize();
        return set;
    }

    public override bool CanConvert(Type objectType) {
        return typeof(SerializedStatSet).IsAssignableFrom(objectType);
    }
}
