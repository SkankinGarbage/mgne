using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

#pragma warning disable 0649

/**
 * For our purposes, a CharaEvent is anything that's going to be moving around the map
 * or has a physical appearance. For parallel process or whatevers, they won't have this.
 */
[RequireComponent(typeof(MapEvent))]
[DisallowMultipleComponent]
public class CharaEvent : MonoBehaviour {

    private const string DefaultMaterial2DPath = "Materials/Sprite2D";
    private const float DesaturationDuration = 0.5f;
    private const float StepsPerSecond = 4.0f;

    [SerializeField] private GameObject doll;
    [SerializeField] private new SpriteRenderer renderer;

    public bool alwaysAnimates;
    public bool cameraRelativeFacing;

    private Dictionary<string, Sprite> sprites;
    private Vector2 lastPosition;
    private bool wasSteppingLastFrame;
    private Vector3 targetPx;
    private float moveTime;
    private bool stepping;

    public MapEvent parent { get { return GetComponent<MapEvent>(); } }
    public Map map { get { return parent.parent; } }
    public float desaturation { get; set; }

    [SerializeField]
    [HideInInspector]
    private Texture2D _spritesheet;
    public Texture2D spritesheet {
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
    public OrthoDir facing {
        get { return _facing; }
        set {
            _facing = value;
            UpdateAppearance();
        }
    }

    private List<SpriteRenderer> _renderers;
    protected List<SpriteRenderer> renderers {
        get {
            if (_renderers == null) {
                _renderers = new List<SpriteRenderer>();
                _renderers.Add(renderer);
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
            facing = (OrthoDir)payload;
        });
        GetComponent<Dispatch>().RegisterListener(MapEvent.EventEnabled, (object payload) => {
            bool enabled = (bool)payload;
            foreach (SpriteRenderer renderer in renderers) {
                renderer.enabled = enabled;
            }
        });
        GetComponent<Dispatch>().RegisterListener(MapEvent.EventInteract, (object payload) => {
            facing = parent.DirectionTo(Global.Instance().Maps.avatar.GetComponent<MapEvent>());
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
        if (spritesheet != null) {
            if (sprites == null || sprites.Count == 0) {
                LoadSpritesheetData();
            }
            renderer.sprite = SpriteForMain();
        }
    }

    public void FaceToward(MapEvent other) {
        facing = parent.DirectionTo(other);
    }

    public Sprite FrameBySlot(int x) {
        return FrameByExplicitSlot(x, facing.Ordinal());
    }
    public Sprite FrameByExplicitSlot(int x, int y) {
        string name = NameForFrame(spritesheet.name, x, y);
        if (!sprites.ContainsKey(name)) {
            Debug.LogError(this + " doesn't contain frame " + name);
        }
        return sprites[name];
    }

    private void CopyShaderValues() {
        foreach (SpriteRenderer renderer in renderers) {
            Material material = Application.isPlaying ? renderer.material : renderer.sharedMaterial;
            if (material != null) {
                material.SetFloat("_Desaturation", desaturation);
            } 
        }
    }

    public IEnumerator StepRoutine(OrthoDir dir) {
        facing = dir;
        Vector2Int offset = parent.OffsetForTiles(dir);
        Vector3 startPx = parent.positionPx;
        targetPx = parent.OwnTileToWorld(parent.position);
        yield return parent.LinearStepRoutine(dir);
    }

    public IEnumerator DesaturateRoutine(float targetDesat) {
        float oldDesat = desaturation;
        float elapsed = 0.0f;
        while (desaturation != targetDesat) {
            elapsed += Time.deltaTime;
            desaturation = Mathf.Lerp(oldDesat, targetDesat, elapsed / DesaturationDuration);
            yield return null;
        }
    }

    private bool IsSteppingThisFrame() {
        Vector2 position = transform.position;
        Vector2 delta = position - lastPosition;
        return alwaysAnimates || (delta.sqrMagnitude > 0 && delta.sqrMagnitude < Map.TileSizePx) || parent.tracking ||
            (GetComponent<AvatarEvent>() && GetComponent<AvatarEvent>().wantsToTrack);
    }

    private void LoadSpritesheetData() {
        string path = DefaultMaterial2DPath;
        foreach (SpriteRenderer renderer in renderers) {
            if (renderer.material == null) {
                renderer.material = Resources.Load<Material>(path);
            }
        }

        sprites = new Dictionary<string, Sprite>();
        path = AssetDatabase.GetAssetPath(spritesheet);
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
