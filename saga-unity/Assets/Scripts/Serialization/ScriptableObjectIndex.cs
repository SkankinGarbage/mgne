using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class ScriptableObjectIndex<T> : GenericIndex<T> where T : ScriptableObject, IKeyedDataObject {

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

    public void PopulateIndex() {
        var selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        var localPath = selectedPath.Substring(0, selectedPath.LastIndexOf('/'));
        RecursivelyPopulateFrom(localPath);
    }
}
