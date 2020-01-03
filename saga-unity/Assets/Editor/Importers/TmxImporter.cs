using UnityEngine;
using System.Collections;
using SuperTiled2Unity.Editor;
using SuperTiled2Unity;

[AutoCustomTmxImporter]
public class MyTmxImporter : CustomTmxImporter {

    public override void TmxAssetImported(TmxAssetImportedArgs args) {
        var map = args.ImportedSuperMap;
        var tsxMap = map.gameObject.AddComponent<TsxMap>();
        tsxMap.grid = map.gameObject.GetComponentInChildren<Grid>();
        var objectLayer = map.gameObject.GetComponentInChildren<SuperObjectLayer>();
        tsxMap.objectLayer = objectLayer.gameObject.AddComponent<ObjectLayer>();

        foreach (Transform child in objectLayer.transform) {
            if (child.GetComponent<SuperObject>() != null) {
                var tmxObject = child.GetComponent<SuperObject>();
                var mapEvent = child.gameObject.AddComponent<MapEvent2D>();
                mapEvent.size = new Vector2Int((int)tmxObject.m_Width / 16, (int)tmxObject.m_Height / 16);
                mapEvent.properties = tmxObject.GetComponent<SuperCustomProperties>();

                var appearance = mapEvent.GetProperty(MapEvent.PropertyAppearance);
                if (appearance.Contains("4dir_")) {

                }
            }
        }
    }
}
