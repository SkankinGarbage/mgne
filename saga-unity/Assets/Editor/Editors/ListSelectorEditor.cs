using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ListSelector))]
public class ListSelectorEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var selector = (ListSelector)target;
        if (GUILayout.Button("Autopopulate")) {
            selector.cells = new List<SelectableCell>();
            foreach (Transform child in selector.transform) {
                if (child.GetComponent<SelectableCell>() != null) {
                    selector.cells.Add(child.GetComponent<SelectableCell>());
                }
            }
        }
    }
}
