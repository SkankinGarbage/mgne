using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

public class StatModDeserializer : JsonConverter {

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        throw new NotImplementedException("WriteJson");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        StatSet set = new StatSet();
        StatModData data = serializer.Deserialize<StatModData>(reader);
        foreach (StatEntryData stat in data.stats) {
            set[stat.stat] = stat.value;
        }
        if (data.flags != null) {
            foreach (StatTag flag in data.flags) {
                set[flag] = 1;
            }
        }
        return set;
    }

    public override bool CanConvert(Type objectType) {
        return typeof(StatModData).IsAssignableFrom(objectType);
    }
}
