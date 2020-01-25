using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

public abstract class GenericIndex<T> : ScriptableObject where T : Object, IKeyedDataObject {

    [SerializeField] private List<T> dataObjects;

    private Dictionary<string, T> tagToDataObject;

    public void OnEnable() {
        if (dataObjects == null) {
            return;
        }
        tagToDataObject = new Dictionary<string, T>();
        foreach (T dataObject in dataObjects) {
            tagToDataObject[dataObject.Key] = dataObject;
        }
    }

    public T GetData(string key) {
        if (!tagToDataObject.ContainsKey(key.ToLower())) {
            Debug.LogError("Index " + GetType().Name + " does not contain key\"" + key + "\"");
            return default;
        }
        return tagToDataObject[key.ToLower()];
    }

    public T GetDataOrNull(string tag) {
        if (tagToDataObject.ContainsKey(tag.ToLower())) {
            return GetData(tag);
        } else {
            return default;
        }
    }

    public void PopulateIndex() {
        var selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        var localPath = selectedPath.Substring(0, selectedPath.LastIndexOf('/'));
        RecursivelyPopulateFrom(localPath);
    }

    private void RecursivelyPopulateFrom(string dirPath) {
        dataObjects.Clear();
        foreach (var file in Directory.EnumerateFiles(dirPath)) {
            var asset = AssetDatabase.LoadAssetAtPath<T>(file);
            if (!dataObjects.Contains(asset)) {
                dataObjects.Add(asset);
            }
        }
        foreach (var dir in Directory.EnumerateDirectories(dirPath)) {
            RecursivelyPopulateFrom(dir);
        }
        EditorUtility.SetDirty(this);
    }
}
