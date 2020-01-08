using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Reflection;

public class ScriptableObjectGenerator {

    public const string DataDirectory = "DataJsonTest";
    public const string DatabaseDirectory = "Assets/Resources/Database";

    [MenuItem("MGNE Tools/Convert Json")]
    public static void ConvertJson() {
        var directoryPath = Application.dataPath + "/" + DataDirectory;
        ConvertFromDirectory(directoryPath);
        AssetDatabase.SaveAssets();
    }

    private static void ConvertFromDirectory(string directoryPath) {
        foreach (string filePath in Directory.GetFiles(directoryPath)) {
            ConvertFromFile(filePath);
        }
        foreach (string subdirectoryPath in Directory.GetDirectories(directoryPath)) {
            ConvertFromDirectory(subdirectoryPath);
        }
    }

    private static void ConvertFromFile(string filePath) {
        if (!filePath.EndsWith(".json")) {
            return;
        }

        var jsonString = JsonAtFilepath(filePath);
        var type = TypeForFilepath(filePath);
        if (type != null) {
            ConvertRaw(type, jsonString);
            Link(type, jsonString);
        }
    }

    private static void ConvertRaw(Type type, string jsonString) {
        var obj = JsonConvert.DeserializeObject(jsonString, type);
        var path = PathForJson(type, obj as MainSchema);

        if (!File.Exists(path)) {
            AssetDatabase.CreateAsset((UnityEngine.Object)obj, path);
            AssetDatabase.SaveAssets();
            Debug.Log("Writing to " + path + "...");
        }
    }

    private static void Link(Type type, string jsonString) {
        LinkerDeserializer.UseLinks = true;
        var linkedAsset = (MainSchema) JsonConvert.DeserializeObject(jsonString, type);
        var storedAsset = AssetForJson(type, linkedAsset);

        // copy from the linked version to the one on disk
        var destinationFields = storedAsset.GetType().GetFields();
        foreach (var destinationField in destinationFields) {
            var sourceField = linkedAsset.GetType().GetField(destinationField.Name);
            destinationField.SetValue(storedAsset, sourceField.GetValue(linkedAsset));
        }
    }

    public static Type TypeForFilepath(string filePath) {
        string typeString = filePath.Substring(0, filePath.LastIndexOf('\\'));
        typeString = typeString.Substring(typeString.LastIndexOf('\\') + 1, typeString.Length - typeString.LastIndexOf('\\') - 1);
        typeString = typeString.Replace("MDO", "Data");
        var assembly = typeof(CharaData).Assembly;
        Type type = assembly.GetType(typeString);
        return type;
    }

    public static string PathForJson(Type schemaSubclass, MainSchema mainSchema) {
        var path = "Assets/Resources/Database/" + schemaSubclass.ToString();
        path = path.Substring(0, path.LastIndexOf("Data"));
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        if (mainSchema == null) {
            Debug.LogError("Bad schema " + schemaSubclass);
            return "";
        }
        if (mainSchema.subfolder.Length > 0) {
            path += "/" + mainSchema.subfolder;
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }
        path += "/" + mainSchema.key + ".asset";
        return path;
    }

    public static string JsonAtFilepath(string filepath) {
        FileStream fileReader = File.Open(filepath, FileMode.Open);
        StreamReader streamReader = new StreamReader(fileReader);
        string jsonString = streamReader.ReadToEnd();
        streamReader.Close();
        fileReader.Close();

        return jsonString;
    }

    public static MainSchema AssetForJson(Type schemaSubclass, MainSchema mainSchema) {
        string path = DatabaseDirectory + "/" + schemaSubclass.ToString().Replace("Data", "");
        if (mainSchema.subfolder?.Length > 0) {
            path += "/" + mainSchema.subfolder;
        }
        path += "/" + mainSchema.key + ".asset";
        return AssetDatabase.LoadAssetAtPath<MainSchema>(path);
    }
}
