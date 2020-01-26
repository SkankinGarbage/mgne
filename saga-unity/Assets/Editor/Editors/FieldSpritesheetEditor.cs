using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FieldSpritesheetComponent), editorForChildClasses: true)]
public class FieldSpritesheetEditor : Editor {

    private string newTag = "";

    public override void OnInspectorGUI() {
        var spritesheet = (FieldSpritesheetComponent)target;
        GUILayout.Label("Current spritesheet: " + spritesheet.Name);

        newTag = GUILayout.TextField(newTag);
        if (GUILayout.Button("Set by tag")) {
            newTag = "";
            spritesheet.SetByTag(newTag);
        }
    }
}
