using UnityEngine;
using System.Collections.Generic;

public abstract class GenericIndex<T> : ScriptableObject where T : IKeyedDataObject {

    public List<T> dataObjects;

    private Dictionary<string, T> tagToDataObject;

    public void OnEnable() {
        if (dataObjects == null) {
            return;
        }
        tagToDataObject = new Dictionary<string, T>();
        foreach (T dataObject in dataObjects) {
            tagToDataObject[dataObject.Key] = dataObject;
        }
    }

    public T GetData(string key) {
        if (!tagToDataObject.ContainsKey(key.ToLower())) {
            Debug.LogError("Index " + GetType().Name + " does not contain key\"" + key + "\"");
            return default(T);
        }
        return tagToDataObject[key.ToLower()];
    }

    public T GetDataOrNull(string tag) {
        if (tagToDataObject.ContainsKey(tag.ToLower())) {
            return GetData(tag);
        } else {
            return default(T);
        }
    }
}
