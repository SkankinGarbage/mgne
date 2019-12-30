using UnityEngine;
using System.Collections;
using SuperTiled2Unity;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SuperMap))]
public class TsxMap : Map {

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

    public override PropertiedTile TileAt(Tilemap layer, int x, int y) {
        var genericTile = layer.GetTile(TileToTilemapCoords(x, y));
        TsxTile tsxTile = null;
        instantiatedTiles.TryGetValue(genericTile, out tsxTile);
        if (tsxTile == null) {
            tsxTile = new TsxTile((SuperTile)genericTile);
            instantiatedTiles[genericTile] = tsxTile;
        }
        return tsxTile;
    }
}
