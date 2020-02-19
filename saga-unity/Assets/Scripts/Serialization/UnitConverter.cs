using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class UnitConverter : JsonConverter<Unit> {

    public override void WriteJson(JsonWriter writer, Unit value, JsonSerializer serializer) {
        var toSerialize = new SerializedUnit(value);
        serializer.Serialize(writer, toSerialize);
    }

    public override Unit ReadJson(JsonReader reader, Type objectType, Unit existingValue, bool hasExistingValue, JsonSerializer serializer) {
        JObject item = JObject.Load(reader);
        var serializedUnit = item.ToObject<SerializedUnit>();
        return new Unit(serializedUnit);
    }
}
