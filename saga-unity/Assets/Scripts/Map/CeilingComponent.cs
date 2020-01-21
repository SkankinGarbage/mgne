using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SuperTiled2Unity;
using UnityEditor;
using SuperTiled2Unity.Editor;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CeilingComponent : MonoBehaviour {

    private const string PropertyTileId = "roofID";
    private const string PropertyTileset = "roofTileset";
    private const string TilesetDirectory = "Assets/Resources/Maps/tilesets";
    
    [SerializeField] [HideInInspector] private Vector2[] uvs;
    [SerializeField] private int tileId = 0;
    [SerializeField] private Texture tilesetTexture = null;

    [SerializeField] [HideInInspector] private new PolygonCollider2D collider = null;
    [SerializeField] [HideInInspector] private new MeshRenderer renderer = null;
    [SerializeField] [HideInInspector] private MeshFilter filter = null;
    [SerializeField] [HideInInspector] private MapEvent @event = null;

    private MaterialPropertyBlock propBlock;


    public void Reconfigure(MapEvent parent, TmxAssetImporter importer) {
        collider = parent.GetComponent<PolygonCollider2D>();
        renderer = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
        @event = parent.GetComponent<MapEvent>();

        var props = parent.Properties;
        CustomProperty prop;
        if (props.TryGetCustomProperty(PropertyTileId, out prop)) {
            tileId = prop.GetValueAsInt();
        } else {
            Debug.LogError("Bad ceiling " + parent);
        }
        if (props.TryGetCustomProperty(PropertyTileset, out prop)) {
            var tilesetName = prop.GetValueAsString();
            var path = TilesetDirectory + "/" + tilesetName + ".png";
            tilesetTexture = AssetDatabase.LoadAssetAtPath<Texture>(path);
        } else {
            Debug.LogError("Bad ceiling " + parent);
        }

        RecalculateUVs();
        renderer.material = GameboyMaterialSettings.GetDefault().BackgroundMaterial;

        RecalculateBounds(importer);
    }

    public void Awake() {
        propBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propBlock);
        propBlock.SetTexture("_MainTex", tilesetTexture);
    }

    public void Update() {
        renderer.SetPropertyBlock(propBlock);
    }

    public void RecalculateBounds(TmxAssetImporter importer = null) {
        var bounds = collider.bounds;
        var size = new Vector2Int(  Mathf.CeilToInt(bounds.size.x), 
                                    Mathf.CeilToInt(bounds.size.y)  );

        Mesh mesh;
        if (Application.isPlaying) {
            if (filter.mesh == null) {
                filter.mesh = new Mesh();
            }
            mesh = filter.mesh;
        } else {
            if (filter.sharedMesh == null) {
                filter.sharedMesh = new Mesh();
            }
            mesh = filter.sharedMesh;
            mesh.name = "Ceiling mesh";
            if (importer != null) {
                importer.SuperImportContext.AddObjectToAsset("Ceiling mesh", mesh);
            }
        }

        mesh.Clear();
        var verts = new Vector3[0];
        var tris = new int[0];
        var norms = new Vector3[0];
        var uvs = new Vector2[0];
        for (int x = 0; x <= size.x; x += 1) {
            for (int y = 0; y <= size.y; y +=1) {
                var tile = new Vector2Int(x + @event.Position.x, y + @event.Position.y);
                if (ContainsTile(tile)) {
                    AddTileQuadToMesh(tile, ref verts, ref tris, ref norms, ref uvs);
                }
            }
        }
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.normals = norms;
        mesh.uv = uvs;
    }

    private bool ContainsTile(Vector2Int tile) {
        Vector2 center = new Vector2(   (tile.x + 0.5f) * OrthoDir.East.XY2D().x, 
                                        (tile.y + 0.5f) * OrthoDir.North.XY2D().y     );
        return collider.OverlapPoint(center);
    }

    private void AddTileQuadToMesh(Vector2Int tile, ref Vector3[] verts, ref int[] tris, ref Vector3[] norms, ref Vector2[] uvs) {

        var vertOffset = verts.Length;
        Array.Resize(ref verts, vertOffset + 4);
        var z = transform.position.z;
        var pos = tile - @event.Position;
        verts[vertOffset + 0] = new Vector3(pos.x * Map.UnitsPerTile * OrthoDir.East.Px2DX(), pos.y * Map.UnitsPerTile * OrthoDir.North.Px2DY(), z);
        verts[vertOffset + 1] = new Vector3((pos.x + 1) * Map.UnitsPerTile * OrthoDir.East.Px2DX(), pos.y * Map.UnitsPerTile * OrthoDir.North.Px2DY(), z);
        verts[vertOffset + 2] = new Vector3(pos.x * Map.UnitsPerTile * OrthoDir.East.Px2DX(), (pos.y + 1) * Map.UnitsPerTile * OrthoDir.North.Px2DY(), z);
        verts[vertOffset + 3] = new Vector3((pos.x + 1) * Map.UnitsPerTile * OrthoDir.East.Px2DX(), (pos.y + 1) * Map.UnitsPerTile * OrthoDir.North.Px2DY(), z);

        var trisOffset = tris.Length;
        Array.Resize(ref tris, trisOffset + 6);
        tris[trisOffset + 0] = vertOffset + 0;
        tris[trisOffset + 1] = vertOffset + 2;
        tris[trisOffset + 2] = vertOffset + 1;
        tris[trisOffset + 3] = vertOffset + 2;
        tris[trisOffset + 4] = vertOffset + 3;
        tris[trisOffset + 5] = vertOffset + 1;

        var normsOffset = norms.Length;
        Array.Resize(ref norms, normsOffset + 4);
        norms[normsOffset + 0] = -Vector3.forward;
        norms[normsOffset + 1] = -Vector3.forward;
        norms[normsOffset + 2] = -Vector3.forward;
        norms[normsOffset + 3] = -Vector3.forward;

        var uvsOffset = uvs.Length;
        Array.Resize(ref uvs, uvsOffset + 4);
        uvs[uvsOffset + 0] = this.uvs[0];
        uvs[uvsOffset + 1] = this.uvs[1];
        uvs[uvsOffset + 2] = this.uvs[2];
        uvs[uvsOffset + 3] = this.uvs[3];
    }

    private void RecalculateUVs() {
        var rowCount = tilesetTexture.width / Map.PxPerTile;
        var colCount = tilesetTexture.height / Map.PxPerTile;
        var row = tileId / rowCount;
        var col = tileId % rowCount;

        var @base = new Vector2(row / rowCount, col / colCount);
        uvs = new Vector2[4];
        uvs[0] = new Vector2(@base.x, @base.y);
        uvs[1] = new Vector2(@base.x + (float)Map.PxPerTile / tilesetTexture.width, @base.y);
        uvs[2] = new Vector2(@base.x, @base.y + (float)Map.PxPerTile / tilesetTexture.height);
        uvs[3] = new Vector2(@base.x + (float)Map.PxPerTile / tilesetTexture.width, @base.y + (float)Map.PxPerTile / tilesetTexture.height);
    }
}
