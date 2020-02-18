using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ScriptableObjectIndex<T> : GenericIndex<T>, IIndexPopulater where T : ScriptableObject, IKeyedDataObject {

    private void RecursivelyPopulateFrom(string dirPath) {
        foreach (var file in Directory.EnumerateFiles(dirPath)) {
            var asset = AssetDatabase.LoadAssetAtPath<T>(file);
            if (asset != null) {
                dataObjects.Add(asset);
            }
        }
        foreach (var dir in Directory.EnumerateDirectories(dirPath)) {
            RecursivelyPopulateFrom(dir);
        }
        EditorUtility.SetDirty(this);
    }

    public void PopulateIndex() {
        if (dataObjects == null) {
            dataObjects = new List<T>();
        } else {
            dataObjects.Clear();
        }
        var selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        var localPath = selectedPath.Substring(0, selectedPath.LastIndexOf('/'));
        RecursivelyPopulateFrom(localPath);
    }
}
