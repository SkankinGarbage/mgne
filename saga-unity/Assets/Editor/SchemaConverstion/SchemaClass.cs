using UnityEngine;
using System.Collections;

/// <summary>
/// Deserialized from JSON from the original MGNE
/// </summary>
public class SchemaClass {

    public string name;
    public bool excludeFromTree = false;
    public string path = "";

    public SchemaField[] fields;

}
