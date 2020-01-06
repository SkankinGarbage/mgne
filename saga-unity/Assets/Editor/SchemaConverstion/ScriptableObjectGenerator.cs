using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using Newtonsoft.Json;

public class ScriptableObjectGenerator {

    private const string DataDirectory = "DataJsonTest";

    [MenuItem("MGNE Tools/Convert Json")]
    public static void ConvertJson() {
        var directoryPath = Application.dataPath + "/" + DataDirectory;
        ConvertFromDirectory(directoryPath);
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

        FileStream fileReader = File.Open(filePath, FileMode.Open);
        StreamReader streamReader = new StreamReader(fileReader);
        string jsonString = streamReader.ReadToEnd();
        streamReader.Close();
        fileReader.Close();

        string typeString = filePath.Substring(0, filePath.LastIndexOf('\\'));
        typeString = typeString.Substring(typeString.LastIndexOf('\\') + 1, typeString.Length - typeString.LastIndexOf('\\') - 1);
        typeString = typeString.Replace("MDO", "Data");
        var assembly = typeof(CharaData).Assembly;
        Type type = assembly.GetType(typeString);
        if (type != null) {
            ConvertRaw(type, jsonString);
        }
    }

    private static void ConvertRaw(Type type, string jsonString) {
        var obj = JsonConvert.DeserializeObject(jsonString, type);
        var path = "Assets/Resources/Database/" + type.ToString();
        path = path.Substring(0, path.LastIndexOf("Data"));
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        var mainSchema = obj as MainSchema;
        if (mainSchema == null) {
            Debug.LogError("Bad schema " + type);
        }
        if (mainSchema.subfolder.Length > 0) {
            path += "/" + mainSchema.subfolder;
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }
        path += "/" + mainSchema.key + ".asset";
        
        if (!File.Exists(path)) {
            AssetDatabase.CreateAsset((UnityEngine.Object) obj, path);
            Debug.Log("Writing to " + path + "...");
        }
    }
}
