using UnityEngine;
using System.Collections;
using SuperTiled2Unity;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SuperMap))]
public class TsxMap : Map {

    private const string PropertyBgmKey = "bgm";
    private const string PropertyName = "name";

    private static Dictionary<TileBase, TsxTile> instantiatedTiles 
        = new Dictionary<TileBase, TsxTile>();

    private SuperMap _tsx;
    public SuperMap Tsx {
        get {
            if (_tsx == null) {
                _tsx = GetComponent<SuperMap>();
            }
            return _tsx;
        }
    }

    public override void Start() {
        base.Start();
        BgmKey = GetProperty(PropertyBgmKey);
        MapName = GetProperty(PropertyName);
    }

    public override PropertiedTile TileAt(Tilemap layer, int x, int y) {
        var genericTile = layer.GetTile(TileToTilemapCoords(x, y));
        if (genericTile == null) {
            return null;
        }
        TsxTile tsxTile = null;
        instantiatedTiles.TryGetValue(genericTile, out tsxTile);
        if (tsxTile == null) {
            tsxTile = new TsxTile((SuperTile)genericTile);
            instantiatedTiles[genericTile] = tsxTile;
        }
        return tsxTile;
    }

    protected override Vector2Int InternalGetSize() {
        return new Vector2Int(Tsx.m_Width, Tsx.m_Height);
    }

    private SuperCustomProperties props;
    protected string GetProperty(string propertyName) {
        if (props == null) {
            props = GetComponent<SuperCustomProperties>();
        }
        CustomProperty property;
        props.TryGetCustomProperty(propertyName, out property);
        return property?.GetValueAsString();
    }
}
