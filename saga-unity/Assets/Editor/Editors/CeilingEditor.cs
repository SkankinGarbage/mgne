using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CeilingComponent))]
public class CeilingEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        CeilingComponent ceiling = (CeilingComponent)target;
        if (GUILayout.Button("Recalculate bounds")) {
            ceiling.RecalculateBounds();
            EditorUtility.SetDirty(ceiling);
        }
    }
}
