using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class InventoryConverter : JsonConverter<Inventory> {

    public override void WriteJson(JsonWriter writer, Inventory value, JsonSerializer serializer) {
        var toSerialize = new List<SerializedCombatItem>();
        foreach (var item in value) {
            if (item == null) {
                toSerialize.Add(null);
            } else {
                toSerialize.Add(new SerializedCombatItem(item));
            }
            
        }
        serializer.Serialize(writer, toSerialize);
    }

    public override Inventory ReadJson(JsonReader reader, Type objectType, Inventory existingValue, bool hasExistingValue, JsonSerializer serializer) {
        var serialized = serializer.Deserialize<List<SerializedCombatItem>>(reader);
        return new Inventory(serialized);
    }
}
