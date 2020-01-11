using UnityEngine;
using System.Collections;
using SuperTiled2Unity.Editor;
using SuperTiled2Unity;
using UnityEditor;

[AutoCustomTmxImporter]
public class MyTmxImporter : CustomTmxImporter {

    private string dollPrefabPath = "Assets/Resources/Prefabs/Doll.prefab";

    public override void TmxAssetImported(TmxAssetImportedArgs args) {
        var map = args.ImportedSuperMap;
        var tsxMap = map.gameObject.AddComponent<TsxMap>();
        tsxMap.grid = map.gameObject.GetComponentInChildren<Grid>();
        var objectLayer = map.gameObject.GetComponentInChildren<SuperObjectLayer>();
        tsxMap.objectLayer = objectLayer.gameObject.AddComponent<ObjectLayer>();

        foreach (Transform child in objectLayer.transform) {
            if (child.GetComponent<SuperObject>() != null) {
                var tmxObject = child.GetComponent<SuperObject>();
                child.gameObject.AddComponent<MapEvent2D>();
                var mapEvent = child.gameObject.GetComponent<MapEvent2D>();
                mapEvent.size = new Vector2Int((int)tmxObject.m_Width / 16, (int)tmxObject.m_Height / 16);
                mapEvent.properties = tmxObject.GetComponent<SuperCustomProperties>();

                var appearance = mapEvent.GetProperty(MapEvent.PropertyAppearance);
                if (appearance != null && appearance.Length > 0) {
                    CharaEvent chara;
                    Doll doll;
                    if (mapEvent.GetComponent<CharaEvent>() == null) {
                        chara = mapEvent.gameObject.AddComponent<CharaEvent>();
                        var dollObject = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(dollPrefabPath));
                        doll = dollObject.GetComponent<Doll>();
                        doll.transform.SetParent(mapEvent.transform);
                        chara.Doll = doll;
                    } else {
                        chara = mapEvent.GetComponent<CharaEvent>();
                        doll = chara.Doll;
                    }

                    doll.transform.localPosition = Vector3.zero;
                    
                    var facing = mapEvent.GetProperty("face");
                    if (facing != null && facing.Length > 0) {
                        chara.Facing = OrthoDirExtensions.Parse(facing);
                    }

                    if (IndexDatabase.Instance().FieldSprites.GetDataOrNull(appearance) != null) {
                        // it's a literal
                        chara.SetAppearanceByTag(appearance);
                    }
                }
            }
        }
    }
}
