using UnityEngine;
using System.Collections;

public class MainSchema : ScriptableObject, IKeyedDataObject {

    public string subfolder;

    public string key;
    public string Key { get { return key; } }
}
