﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

#pragma warning disable 0649

/**
 * For our purposes, a CharaEvent is anything that's going to be moving around the map
 * or has a physical appearance. For parallel process or whatevers, they won't have this.
 */
[DisallowMultipleComponent]
public class CharaEvent : MonoBehaviour {

    private const string DefaultMaterial2DPath = "Materials/Sprite2D";
    private const float DesaturationDuration = 0.5f;
    private const float StepsPerSecond = 4.0f;

    public Doll Doll;

    [SerializeField] private bool alwaysAnimates = true;

    private Dictionary<string, Sprite> sprites;
    private Vector2 lastPosition;
    private bool wasSteppingLastFrame;
    private Vector3 targetPx;
    private float moveTime;
    private bool stepping;

    public MapEvent Parent { get { return GetComponent<MapEvent>(); } }
    public Map Map { get { return Parent.parent; } }
    public float Desaturation { get; set; }

    [SerializeField]
    private Texture2D _spritesheet;
    public Texture2D Spritesheet {
        get { return _spritesheet; }
        set {
            _spritesheet = value;
            LoadSpritesheetData();
            UpdateAppearance();
        }
    }

    [SerializeField]
    [HideInInspector]
    private OrthoDir _facing = OrthoDir.South;
    public OrthoDir Facing {
        get { return _facing; }
        set {
            _facing = value;
            UpdateAppearance();
        }
    }

    private List<SpriteRenderer> _renderers;
    protected List<SpriteRenderer> Renderers {
        get {
            if (_renderers == null) {
                _renderers = new List<SpriteRenderer>();
                _renderers.Add(Doll.Renderer);
            }
            return _renderers;
        }
    }

    public static string NameForFrame(string sheetName, int x, int y) {
        return sheetName + "_" + x + "_" + y;
    }

    public void Start() {
        CopyShaderValues();
        GetComponent<Dispatch>().RegisterListener(MapEvent.EventMove, (object payload) => {
            Facing = (OrthoDir)payload;
        });
        GetComponent<Dispatch>().RegisterListener(MapEvent.EventEnabled, (object payload) => {
            bool enabled = (bool)payload;
            foreach (SpriteRenderer renderer in Renderers) {
                renderer.enabled = enabled;
            }
        });
        GetComponent<Dispatch>().RegisterListener(MapEvent.EventInteract, (object payload) => {
            Facing = Parent.DirectionTo(Global.Instance().Maps.avatar.GetComponent<MapEvent>());
        });
    }

    public void Update() {
        CopyShaderValues();

        bool steppingThisFrame = IsSteppingThisFrame();
        stepping = steppingThisFrame || wasSteppingLastFrame;
        if (!steppingThisFrame && !wasSteppingLastFrame) {
            moveTime = 0.0f;
        } else {
            moveTime += Time.deltaTime;
        }
        wasSteppingLastFrame = steppingThisFrame;
        lastPosition = transform.position;

        UpdateAppearance();
    }

    public void UpdateAppearance() {
        if (Spritesheet != null) {
            if (sprites == null || sprites.Count == 0) {
                LoadSpritesheetData();
            }
            Doll.Renderer.sprite = SpriteForMain();
        }
    }

    public void FaceToward(MapEvent other) {
        Facing = Parent.DirectionTo(other);
    }

    public Sprite FrameBySlot(int x) {
        return FrameByExplicitSlot(x, Facing.Ordinal());
    }
    public Sprite FrameByExplicitSlot(int x, int y) {
        string name = NameForFrame(Spritesheet.name, x, y);
        if (!sprites.ContainsKey(name)) {
            Debug.LogError(this + " doesn't contain frame " + name);
        }
        return sprites[name];
    }

    public void SetAppearanceByTag(string fieldSpriteTag) {
        Spritesheet = IndexDatabase.Instance().FieldSprites.GetData(fieldSpriteTag).spriteSheet;
    }

    private void CopyShaderValues() {
        foreach (SpriteRenderer renderer in Renderers) {
            Material material = Application.isPlaying ? renderer.material : renderer.sharedMaterial;
            if (material != null) {
                material.SetFloat("_Desaturation", Desaturation);
            } 
        }
    }

    public IEnumerator StepRoutine(OrthoDir dir) {
        Facing = dir;
        Vector2Int offset = Parent.OffsetForTiles(dir);
        Vector3 startPx = Parent.positionPx;
        targetPx = Parent.OwnTileToWorld(Parent.position);
        yield return Parent.LinearStepRoutine(dir);
    }

    public IEnumerator DesaturateRoutine(float targetDesat) {
        float oldDesat = Desaturation;
        float elapsed = 0.0f;
        while (Desaturation != targetDesat) {
            elapsed += Time.deltaTime;
            Desaturation = Mathf.Lerp(oldDesat, targetDesat, elapsed / DesaturationDuration);
            yield return null;
        }
    }

    private bool IsSteppingThisFrame() {
        Vector2 position = transform.position;
        Vector2 delta = position - lastPosition;
        return alwaysAnimates || (delta.sqrMagnitude > 0 && delta.sqrMagnitude < Map.TileSizePx) || Parent.tracking ||
            (GetComponent<AvatarEvent>() && GetComponent<AvatarEvent>().wantsToTrack);
    }

    private void LoadSpritesheetData() {
        string path = DefaultMaterial2DPath;

        sprites = new Dictionary<string, Sprite>();
        path = AssetDatabase.GetAssetPath(Spritesheet);
        if (path.StartsWith("Assets/Resources/")) {
            path = path.Substring("Assets/Resources/".Length);
        }
        if (path.EndsWith(".png")) {
            path = path.Substring(0, path.Length - ".png".Length);
        }
        foreach (Sprite sprite in Resources.LoadAll<Sprite>(path)) {
            sprites[sprite.name] = sprite;
        }
    }

    private Sprite SpriteForMain() {
        int x = Mathf.FloorToInt(moveTime * StepsPerSecond) % 4;
        if (x == 3) x = 1;
        if (!stepping) x = 1;
        return FrameBySlot(x);
    }
}
