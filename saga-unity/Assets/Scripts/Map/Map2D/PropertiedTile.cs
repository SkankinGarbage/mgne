using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class PropertiedTile {

    #region Abstract

    public abstract bool EqualsTile(TileBase tile);

    protected abstract string GetProperty(string propertyName);

    #endregion

    #region Properties

    private readonly string PropertyPassable = "o";
    private readonly string PropertyImpassable = "x";

    public bool IsPassable => GetProperty(PropertyPassable) != null;
    public bool IsImpassable => GetProperty(PropertyImpassable) != null;

    #endregion
}
