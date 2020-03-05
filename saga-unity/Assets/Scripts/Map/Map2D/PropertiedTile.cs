using UnityEngine.Tilemaps;

public abstract class PropertiedTile {

    #region Abstract

    public abstract bool EqualsTile(TileBase tile);

    protected abstract string GetProperty(string propertyName);

    #endregion

    #region Properties

    private const string PropertyPassable = "o";
    private const string PropertyImpassable = "x";
    private const string PropertyCounter = "counter";

    public bool IsPassable => GetProperty(PropertyPassable) != null;
    public bool IsImpassable => GetProperty(PropertyImpassable) != null;
    public bool IsCounter => GetProperty(PropertyCounter) != null;

    #endregion
}
