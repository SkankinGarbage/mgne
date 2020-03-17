using UnityEditor;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using UnityEngine;

public class DumpSaveMenu {

    [MenuItem("MGNE Tools/Dump save")]
    public static void ConvertSchema() {
        DumpSave();
    }

    public static void DumpSave() {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new StatSetConverter());
        settings.Converters.Add(new UnitConverter());
        settings.Converters.Add(new InventoryConverter());
        var serializer = JsonSerializer.Create(settings);

        var builder = new StringBuilder();
        var stringWriter = new StringWriter(builder);
        var writer = new JsonTextWriter(stringWriter);
        serializer.Serialize(writer, Global.Instance().Data);

        Debug.Log(builder.ToString());
    }
}
