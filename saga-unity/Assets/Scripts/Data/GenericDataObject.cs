using UnityEngine;

public abstract class GenericDataObject : Object, IKeyedDataObject {

    [SerializeField] private string tag = null;
    public string Key { get { return tag; } }

}
