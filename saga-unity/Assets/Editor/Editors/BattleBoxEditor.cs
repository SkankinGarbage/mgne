using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BattleBox), editorForChildClasses:true)]
public class BattleBoxEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var box = (BattleBox)target;

        if (Application.isPlaying && GUILayout.Button("Test")) {
            box.StartCoroutine(box.TestRoutine());
        }
        if (Application.isPlaying && GUILayout.Button("Clear")) {
            box.Clear();
        }
    }
}
