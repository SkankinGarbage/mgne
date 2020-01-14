using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Textbox), editorForChildClasses:true)]
public class TextboxEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Textbox box = (Textbox)target;
        if (GUILayout.Button("Hide")) {
            box.Hide();
        }

        if (GUILayout.Button("Show")) {
            box.Show();
        }

        if (GUILayout.Button("Memorize sizes")) {
            box.MemorizeSizes();
            EditorUtility.SetDirty(box);
        }

        if (GUILayout.Button("Test")) {
            box.StartCoroutine(box.TestRoutine());
        }
    }
}
