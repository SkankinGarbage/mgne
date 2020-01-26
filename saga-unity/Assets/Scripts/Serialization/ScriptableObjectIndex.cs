using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

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
        dataObjects.Clear();
        var selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        var localPath = selectedPath.Substring(0, selectedPath.LastIndexOf('/'));
        RecursivelyPopulateFrom(localPath);
    }
}
