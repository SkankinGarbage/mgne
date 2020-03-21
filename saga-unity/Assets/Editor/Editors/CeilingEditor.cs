using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CeilingComponent))]
public class CeilingEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        CeilingComponent ceiling = (CeilingComponent)target;
        if (GUILayout.Button("Recalculate bounds")) {
            ceiling.RecalculateMesh();
            EditorUtility.SetDirty(ceiling);
        }
        if (GUILayout.Button("Debug bounds")) {
            ceiling.DebugBounds();
        }
    }
}
