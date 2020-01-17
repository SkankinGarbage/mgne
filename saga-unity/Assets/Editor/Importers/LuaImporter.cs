using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;

[ScriptedImporter(1, new string[] { "scene", "lua" })]
public class CubeImporter : ScriptedImporter {

    public override void OnImportAsset(AssetImportContext context) {
        var script = ScriptableObject.CreateInstance<LuaSerializedScript>();
        var text = File.ReadAllText(context.assetPath);
        script.luaString = text;
        context.AddObjectToAsset("Script", script);
        context.SetMainObject(script);
    }
}
