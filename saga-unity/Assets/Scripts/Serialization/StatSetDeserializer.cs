using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

public class StatSetDeserializer : JsonConverter {

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        throw new NotImplementedException("WriteJson");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        StatSet set = new StatSet();
        StatSetData data = serializer.Deserialize<StatSetData>(reader);
        set[StatTag.STR] = data.str;
        set[StatTag.DEF] = data.def;
        set[StatTag.AGI] = data.agi;
        set[StatTag.MANA] = data.mana;
        set[StatTag.HP] = data.hp;
        set[StatTag.MHP] = data.mhp;
        foreach (StatTag flag in data.flags) {
            set[flag] = 1;
        }
        return set;
    }

    public override bool CanConvert(Type objectType) {
        return typeof(StatSetData).IsAssignableFrom(objectType);
    }
}
