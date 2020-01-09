using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Reflection;
using System.Linq;

public class PolymorphicFieldUtility {

    private string DatabaseDirectory = "Assets/Resources/Database";

    private Type baseType, parentType;
    private PolymorphicSchema currentValue;
    private Dictionary<string, Type> cachedSubclasses;

    public PolymorphicFieldUtility(Type baseType, Type parentType, PolymorphicSchema existingValue) {
        this.baseType = baseType;
        this.parentType = parentType;
        currentValue = existingValue;
    }

    public T DrawSelector<T>(T obj) where T : PolymorphicSchema {
        string[] names = GetSubclasses().Keys.ToArray();

        int index = 0;
        if (obj != null) {
            index = GetSubclasses().Values.ToList().IndexOf(obj.GetType());
        }
        int selectedIndex = EditorGUILayout.Popup(baseType.Name, index, names);

        if (selectedIndex != index) {
            if (obj != null) {
                AssetDatabase.DeleteAsset(PathForTarget(currentValue.key));
                return null;
            }
            if (selectedIndex != 0) {
                Type type = GetSubclasses().Values.ToArray()[selectedIndex];
                var newKey = GenerateNewKey(type);
                var instance = ScriptableObject.CreateInstance(type);
                currentValue = (PolymorphicSchema) instance;
                currentValue.key = newKey;
                AssetDatabase.CreateAsset(instance, PathForTarget(newKey));
                return (T) instance;
            }
        }
        return obj;
    }

    protected Dictionary<string, Type> GetSubclasses() {
        if (cachedSubclasses == null) {
            cachedSubclasses = new Dictionary<string, Type> {
                { "(none)", null }
            };
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type type in ass.GetTypes()) {
                    if (baseType.IsAssignableFrom(type) && !type.IsAbstract) {
                        cachedSubclasses.Add(type.Name, type);
                    }
                }
            }
        }
        return cachedSubclasses;
    }

    protected string PathForTarget(string name) {
        return DatabaseDirectory + "/" + currentValue.GetType().Name + "/" + currentValue.key;
    }

    protected string GenerateNewKey(Type newType) {
        return "anon_" + parentType.Name + UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }
}
