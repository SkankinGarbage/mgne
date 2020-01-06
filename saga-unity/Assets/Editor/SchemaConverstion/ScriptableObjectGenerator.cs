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

        string type = filePath.Substring(0, filePath.LastIndexOf('\\'));
        type = type.Substring(type.LastIndexOf('\\') + 1, type.Length - type.LastIndexOf('\\') - 1);
        type = type.Replace("MDO", "Data");
        Debug.Log(type);

        ConvertRaw(type, jsonString);
    }

    private static void ConvertRaw(string typeString, string jsonString) {
        var assembly = typeof(CharaData).Assembly;
        var type = assembly.GetType(typeString);
        var obj = JsonConvert.DeserializeObject(jsonString, type);
        Debug.Log("got here");
    }
}
