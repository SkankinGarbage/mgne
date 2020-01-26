﻿using UnityEngine;
using System.Collections;
using SuperTiled2Unity.Editor;
using SuperTiled2Unity;
using UnityEditor;
using UnityEngine.Tilemaps;

[AutoCustomTmxImporter]
public class MyTmxImporter : CustomTmxImporter {

    private const string DollPrefabPath = "Assets/Resources/Prefabs/Doll.prefab";
    private const string CeilingPrefabPath = "Assets/Resources/Prefabs/Ceiling.prefab";

    private const string TypeCeiling = "Ceiling";

    public override void TmxAssetImported(TmxAssetImportedArgs args) {
        var materials = GameboyMaterialSettings.GetDefault();

        var map = args.ImportedSuperMap;
        var tsxMap = map.gameObject.AddComponent<TsxMap>();
        tsxMap.grid = map.gameObject.GetComponentInChildren<Grid>();
        var objectLayer = map.gameObject.GetComponentInChildren<SuperObjectLayer>();
        if (objectLayer == null) return;
        tsxMap.objectLayer = objectLayer.gameObject.AddComponent<ObjectLayer>();

        foreach (var layer in tsxMap.layers) {
            layer.GetComponent<TilemapRenderer>().material = materials.BackgroundMaterial;
        }

        foreach (Transform child in objectLayer.transform) {
            if (child.GetComponent<SuperObject>() != null) {
                var tmxObject = child.GetComponent<SuperObject>();
                child.gameObject.AddComponent<MapEvent2D>();
                var mapEvent = child.gameObject.GetComponent<MapEvent2D>();
                mapEvent.Size = new Vector2Int((int)tmxObject.m_Width / Map.PxPerTile, (int)tmxObject.m_Height / Map.PxPerTile);
                mapEvent.Properties = tmxObject.GetComponent<SuperCustomProperties>();
                mapEvent.Position = new Vector2Int((int)tmxObject.m_X / Map.PxPerTile, (int)tmxObject.m_Y / Map.PxPerTile);

                var appearance = mapEvent.GetProperty(MapEvent.PropertyAppearance);
                if (appearance != null && appearance.Length > 0) {
                    CharaEvent chara;
                    Doll doll;
                    if (mapEvent.GetComponent<FieldSpritesheetComponent>() == null) {
                        mapEvent.gameObject.AddComponent<FieldSpritesheetComponent>();
                    }
                    if (mapEvent.GetComponent<CharaEvent>() == null) {
                        chara = mapEvent.gameObject.AddComponent<CharaEvent>();
                        var dollObject = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(DollPrefabPath));
                        doll = dollObject.GetComponent<Doll>();
                        doll.Renderer.material = materials.ForegroundMaterial;
                        doll.transform.SetParent(mapEvent.transform);
                        chara.Doll = doll;
                    } else {
                        chara = mapEvent.GetComponent<CharaEvent>();
                        doll = chara.Doll;
                    }

                    doll.transform.localPosition = Vector3.zero;

                    if (IndexDatabase.Instance().FieldSprites.GetDataOrNull(appearance) != null) {
                        // it's a literal
                        chara.SetAppearanceByTag(appearance);
                    }

                    var facing = mapEvent.GetProperty("face");
                    if (facing != null && facing.Length > 0) {
                        chara.Facing = OrthoDirExtensions.Parse(facing);
                    }
                }

                if (tmxObject.m_Type == TypeCeiling) {
                    CeilingComponent ceil = mapEvent.GetComponentInChildren<CeilingComponent>();
                    if (ceil == null) {
                        var ceilingObject = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(CeilingPrefabPath));
                        ceilingObject.layer = LayerMask.NameToLayer("Ceiling");
                        ceil = ceilingObject.GetComponent<CeilingComponent>();
                        ceilingObject.transform.SetParent(mapEvent.transform);
                        ceilingObject.transform.localPosition = new Vector3(0, 0, -1);
                    }
                    ceil.Reconfigure(mapEvent, args.AssetImporter);
                }
            }
        }
    }
}
