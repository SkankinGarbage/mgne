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
    }
}
