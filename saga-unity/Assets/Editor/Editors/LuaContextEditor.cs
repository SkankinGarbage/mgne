using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LuaComponent))]
public class LuaContextEditor : Editor {

    private string customLua;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        LuaComponent component = (LuaComponent)target;
        LuaContext context = component.Context;
        
        if (Application.IsPlaying(component)) {
            GUILayout.Space(12);

            if (!context.IsRunning()) {
                EditorGUILayout.LabelField("Lua debug prompt!");
            } else {
                EditorGUILayout.LabelField("Running...");
                EditorGUI.BeginDisabledGroup(true);
            }
            
            customLua = EditorGUILayout.TextArea(customLua, new GUILayoutOption[] { GUILayout.Height(120) });
            GUILayout.Space(12);

            if (!context.IsRunning()) {
                if (GUILayout.Button("Run")) {
                    LuaScript script = new LuaScript(context, customLua);
                    component.StartCoroutine(script.RunRoutine());
                }
            } else {
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Force terminate")) {
                    context.ForceTerminate();
                }
            }
            GUILayout.Space(6);
        }
    }

    public override bool RequiresConstantRepaint() {
        LuaComponent component = (LuaComponent)target;
        LuaContext context = component.Context;
        return base.RequiresConstantRepaint() || context.IsRunning();
    }
}
