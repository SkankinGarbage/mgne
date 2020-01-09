using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LinkerDeserializer : JsonConverter {

    public const string DatabaseDirectory = "Resources/Database";

    public static bool UseLinks = false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        throw new NotImplementedException("WriteJson");
    }
    
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
        if (UseLinks) {
            if (reader.TokenType == JsonToken.Null) {
                return null;
            } else if (reader.TokenType == JsonToken.String) {
                var key = serializer.Deserialize<string>(reader);
                return FindAssetForJson(objectType, key);
            } else if (reader.TokenType == JsonToken.StartObject) {
                JObject item = JObject.Load(reader);
                var key = item["key"].Value<string>();
                var javaClassString = item["clazz"].Value<string>();
                var typeString = javaClassString.Substring(javaClassString.LastIndexOf('.') + 1);
                typeString = typeString.Replace("MDO", "Data");
                var type = typeof(CharaData).Assembly.GetType(typeString);
                var asset = FindAssetForJson(type, key);
                return asset;
            } else if (reader.TokenType == JsonToken.StartArray) {
                var keys = serializer.Deserialize<List<string>>(reader);
                List<MainSchema> results = new List<MainSchema>();
                foreach (var key in keys) {
                    var asset = FindAssetForJson(objectType.GetElementType(), key);
                    results.Add(asset);
                }
                var instance = Activator.CreateInstance(objectType, results.Count);
                for (var i = 0; i < results.Count; i+= 1) {
                    ((Array)instance).SetValue(results[i], i);
                }
                return instance;
            } else {
                return null;
            }
        } else {
            if (reader.TokenType == JsonToken.StartObject) {
                JObject.Load(reader);
            }
            if (reader.TokenType == JsonToken.StartArray) {
                JArray.Load(reader);
            }
            return null;
        }

    }

    public override bool CanConvert(Type objectType) {
        return typeof(MainSchema).IsAssignableFrom(objectType);
    }

    public static MainSchema FindAssetForJson(Type schemaSubclass, string key) {
        string dirPath = Application.dataPath + "/" + DatabaseDirectory + "/" + schemaSubclass.ToString().Replace("Data", "");
        
        if (Directory.Exists(dirPath)) {
            return FindAssetForJsonInPath(dirPath, key);
        } else {
            // something like battle anim where it's really a subclass
            foreach (var possibleType in typeof(CharaData).Assembly.GetTypes()) {
                if (!possibleType.IsEquivalentTo(schemaSubclass) && schemaSubclass.IsAssignableFrom(possibleType)) {
                    var result = FindAssetForJson(possibleType, key);
                    if (result != null) {
                        return result;
                    }
                }
            }
        }
        return null;
    }

    private static MainSchema FindAssetForJsonInPath(string dirPath, string key) {
        var targetName = key + ".asset";
        foreach (string filePath in Directory.EnumerateFiles(dirPath)) {
            if (filePath.EndsWith(targetName)) {
                return AssetDatabase.LoadAssetAtPath<MainSchema>(RelativePathForFilePath(filePath));
            }
        }
        foreach (string directoryPath in Directory.EnumerateDirectories(dirPath)) {
            foreach (string filePath in Directory.EnumerateFiles(directoryPath)) {
                if (filePath.EndsWith(targetName)) {
                    return AssetDatabase.LoadAssetAtPath<MainSchema>(RelativePathForFilePath(filePath));
                }
            }
        }
        return null;
    }

    private static string RelativePathForFilePath(string filePath) {
        var prefix = "Assets";
        var relativePath = filePath.Substring(filePath.LastIndexOf(prefix));
        return relativePath;
    }
}
