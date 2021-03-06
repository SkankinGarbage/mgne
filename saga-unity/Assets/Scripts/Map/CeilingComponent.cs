﻿using UnityEngine;
using System.Collections;
using System;
using SuperTiled2Unity;
using UnityEditor;
using System.Text;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CeilingComponent : MonoBehaviour {

    private const string PropertyTileId = "roofID";
    private const string PropertyTileset = "roofTileset";
    private const string TilesetDirectory = "Assets/Resources/Maps/tilesets";

    private const int StepMax = 10;
    private const float Duration = 0.8f;
    
    [SerializeField] [HideInInspector] private Vector2[] uvs;
    [SerializeField] private int tileId = 0;
    [SerializeField] private Texture tilesetTexture = null;

    [SerializeField] [HideInInspector] private new PolygonCollider2D collider = null;
    [SerializeField] [HideInInspector] private new MeshRenderer renderer = null;
    [SerializeField] [HideInInspector] private MeshFilter filter = null;
    [SerializeField] [HideInInspector] private MapEvent @event = null;

    private MaterialPropertyBlock propBlock;

    private bool isWithin;

    private int trisOffset, vertsOffset, uvsOffset, normsOffset;

    #if UNITY_EDITOR

    public void Reconfigure(MapEvent parent, SuperTiled2Unity.Editor.TmxAssetImporter importer) {
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

        RecalculateMesh(importer);
    }

    public void RecalculateMesh(SuperTiled2Unity.Editor.TmxAssetImporter importer = null) {
        var mesh = RecalculateMesh(ContainsTile);
        if (importer != null) {
            var meshName = "Ceiling mesh " + @event.Position.x + "," + @event.Position.y;
            mesh.name = meshName;
            importer.SuperImportContext.AddObjectToAsset(meshName, mesh);
        }
    }

    #endif

    public void Start() {
        propBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propBlock);
        propBlock.SetTexture("_MainTex", tilesetTexture);

        transform.parent.GetComponent<Dispatch>().RegisterListener(MapEvent.EventStep, CheckStep);
    }

    public void Update() {
        renderer.SetPropertyBlock(propBlock);
    }

    public void OnEnable() {
        UpdateImmediate();
        Global.Instance().Dispatch.RegisterListener(MapManager.EventTeleport, OnTeleport);
    }

    public void OnDisable() {
        if (!Global.Destructing) {
            Global.Instance().Dispatch.UnregisterListener(MapManager.EventTeleport, OnTeleport);
        }
    }

    public void OnValidate() {
        renderer.sortingLayerName = "Default";
        renderer.sortingOrder = 6;
    }

    public void OnTeleport(object payload) {
        UpdateImmediate();
    }

    public void UpdateImmediate() {
        if (Global.Instance().Maps.Avatar != null) {
            isWithin = IsHeroWithin();
        }
        if (isWithin) {
            BuildInverted();
        } else {
            BuildStandard();
        }
    }

    public void CheckStep(object payload) {
        if (!isWithin && IsHeroWithin()) {
            StartCoroutine(EnterRoutine());
        } else if (isWithin && !IsHeroWithin()) {
            StartCoroutine(ExitRoutine());
        }
        isWithin = IsHeroWithin();
    }

    public bool IsHeroWithin() {
        return ContainsTile(Global.Instance().Maps.Avatar.Parent.Position);
    }

    public BoundsInt CalculateBounds() {
        var minX = int.MaxValue;
        var maxX = int.MinValue;
        var minY = int.MaxValue;
        var maxY = int.MinValue;
        for (var x = 0; x < @event.Map.Width; x += 1) {
            for (var y = 0; y < @event.Map.Height; y += 1) {
                if (ContainsTile(new Vector2Int(x, y))) {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }
        return new BoundsInt(minX, minY, 0, maxX, maxY, 0);
    }
    
    public Mesh RecalculateMesh(Func<Vector2Int, bool> rule) {
        var bounds = CalculateBounds();
        var size = new Vector2Int(Mathf.CeilToInt(bounds.size.x), Mathf.CeilToInt(bounds.size.y));

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
        }

        mesh.Clear();
        var verts = new Vector3[21 * 21 * size.x * size.y * 4];
        var tris = new int[21 * 21 * size.x * size.y * 6];
        var norms = new Vector3[21 * 21 * size.x * size.y * 4];
        var uvs = new Vector2[21 * 21 * size.x * size.y * 4];
        trisOffset = 0;
        vertsOffset = 0;
        normsOffset = 0;
        uvsOffset = 0;
        for (int x = -StepMax; x <= size.x + StepMax; x += 1) {
            for (int y = -StepMax; y <= size.y + StepMax; y +=1) {
                var tile = new Vector2Int(x + bounds.xMin, y + bounds.yMin);
                if (rule(tile)) {
                    AddTileQuadToMesh(tile, ref verts, ref tris, ref norms, ref uvs);
                }
            }
        }
        var newVerts = new Vector3[vertsOffset];
        var newTris = new int[trisOffset];
        var newNorms = new Vector3[normsOffset];
        var newUvs = new Vector2[uvsOffset];
        Array.Copy(verts, newVerts, vertsOffset);
        Array.Copy(tris, newTris, trisOffset);
        Array.Copy(norms, newNorms, normsOffset);
        Array.Copy(uvs, newUvs, uvsOffset);
        mesh.vertices = newVerts;
        mesh.triangles = newTris;
        mesh.normals = newNorms;
        mesh.uv = newUvs;

        return mesh;
    }

    public void DebugBounds() {
        var output = new StringBuilder();
        var bounds = CalculateBounds();
        var size = new Vector2Int(Mathf.CeilToInt(bounds.size.x),
                                    Mathf.CeilToInt(bounds.size.y));
        for (int y = -StepMax; y <= size.y + StepMax; y += 1) {
            for (int x = -StepMax; x <= size.x + StepMax; x += 1) {
                var tile = new Vector2Int(x + bounds.xMin, y + bounds.yMin);
                if (ContainsTile(tile)) {
                    output.Append("x");
                } else {
                    output.Append("o");
                }
            }
            output.Append("\n");
        }
        Debug.Log(output.ToString());
    }

    private bool ContainsTile(Vector2Int tile) {
        Vector2 center = new Vector2(   (tile.x + 0.6f) * 1, 
                                        (tile.y + 0.6f) * -1     );
        return collider.ClosestPoint(center) == center;
    }
    
    private void AddTileQuadToMesh(Vector2Int tile, ref Vector3[] verts, ref int[] tris, ref Vector3[] norms, ref Vector2[] uvs) {
        
        // optimized for -1 being the orthodir north

        var z = transform.position.z;
        var pos = tile - @event.Position;
        verts[vertsOffset + 0].x = pos.x * Map.UnitsPerTile * 1;
        verts[vertsOffset + 0].y = pos.y * Map.UnitsPerTile * -1;
        verts[vertsOffset + 0].z = z;
        verts[vertsOffset + 1].x = (pos.x + 1) * Map.UnitsPerTile * 1;
        verts[vertsOffset + 1].y = pos.y * Map.UnitsPerTile * -1;
        verts[vertsOffset + 1].z = z;
        verts[vertsOffset + 2].x = pos.x * Map.UnitsPerTile * 1;
        verts[vertsOffset + 2].y = (pos.y + 1) * Map.UnitsPerTile * -1;
        verts[vertsOffset + 2].z = z;
        verts[vertsOffset + 3].x = (pos.x + 1) * Map.UnitsPerTile * 1;
        verts[vertsOffset + 3].y = (pos.y + 1) * Map.UnitsPerTile * -1;
        verts[vertsOffset + 3].z = z;
        
        tris[trisOffset + 0] = vertsOffset + 0;
        tris[trisOffset + 1] = vertsOffset + 2;
        tris[trisOffset + 2] = vertsOffset + 1;
        tris[trisOffset + 3] = vertsOffset + 2;
        tris[trisOffset + 4] = vertsOffset + 3;
        tris[trisOffset + 5] = vertsOffset + 1;
        
        norms[normsOffset + 0] = -Vector3.forward;
        norms[normsOffset + 1] = -Vector3.forward;
        norms[normsOffset + 2] = -Vector3.forward;
        norms[normsOffset + 3] = -Vector3.forward;
        
        uvs[uvsOffset + 0] = this.uvs[0];
        uvs[uvsOffset + 1] = this.uvs[1];
        uvs[uvsOffset + 2] = this.uvs[2];
        uvs[uvsOffset + 3] = this.uvs[3];

        vertsOffset += 4;
        trisOffset += 6;
        normsOffset += 4;
        uvsOffset += 4;
    }

    private void RecalculateUVs() {
        var colCount = tilesetTexture.width / Map.PxPerTile;
        var rowCount = tilesetTexture.height / Map.PxPerTile;
        var col = tileId % colCount;
        var row = tileId / colCount;

        var @base = new Vector2((float)col / colCount, (float)(rowCount - row - 1) / rowCount);
        uvs = new Vector2[4];
        uvs[0] = new Vector2(@base.x, @base.y + (float)Map.PxPerTile / tilesetTexture.height);
        uvs[1] = new Vector2(@base.x + (float)Map.PxPerTile / tilesetTexture.width, @base.y + (float)Map.PxPerTile / tilesetTexture.height);
        uvs[2] = new Vector2(@base.x, @base.y);
        uvs[3] = new Vector2(@base.x + (float)Map.PxPerTile / tilesetTexture.width, @base.y);
    }

    private void BuildStandard() {
        RecalculateMesh(ContainsTile);
    }

    private void BuildInverted() {
        RecalculateMesh(tile => {
            return !ContainsTile(tile);
        });
    }

    private void BuildByRange(Vector2Int at, float reach) {
        RecalculateMesh(tile => {
            var delta = (at - tile);
            if (Math.Abs(delta.x) > Math.Abs(reach) || Math.Abs(delta.y) > Math.Abs(reach)) {
                return ContainsTile(tile) ^ reach > 0;
            }
            return ContainsTile(tile) ^ reach < 0;
        });
    }

    private IEnumerator EnterRoutine() {
        yield return ScaleToRoutine(true);
    }

    private IEnumerator ExitRoutine() {
        yield return ScaleToRoutine(false);
    }

    private IEnumerator ScaleToRoutine(bool invert) {
        Global.Instance().Maps.Avatar.PauseInput();
        float elapsed = 0;
        int atStep = 0;
        while (elapsed < Duration) {
            int newStep = Mathf.FloorToInt(elapsed / Duration * StepMax);
            if (invert) newStep *= -1;
            if (newStep != atStep) {
                atStep = newStep;
                BuildByRange(Global.Instance().Maps.Avatar.Parent.Position, atStep);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (invert) {
            BuildInverted();
        } else {
            BuildStandard();
        }
        Global.Instance().Maps.Avatar.UnpauseInput();
    }
}
