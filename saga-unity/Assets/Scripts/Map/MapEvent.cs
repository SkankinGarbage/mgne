using DG.Tweening;
using SuperTiled2Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * The generic "thing on the map" class for MGNT.
 */
[RequireComponent(typeof(Dispatch))]
[RequireComponent(typeof(LuaCutsceneContext))]
[RequireComponent(typeof(SuperCustomProperties))]
[DisallowMultipleComponent]
public abstract class MapEvent : MonoBehaviour {
    
    public const string EventInteract = "interact";
    public const string EventCollide = "collide";
    public const string EventEnabled = "enabled";
    public const string EventStep = "step";
    
    public const string PropertyLuaHide = "hide";
    public const string PropertyAppearance = "appearance";
    public const string PropertyPassable = "passable";

    public const string PropertyLuaCollide = "onCollide";
    public const string PropertyLuaInteract = "onInteract";
    public const string PropertyLuaAutostart = "onEnter";

    protected const int TilesPerSecond = 4;

    private Vector3 pixelImperfectPos;

    // Editor properties
    public Vector2Int Position = new Vector2Int(0, 0);
    public SuperCustomProperties Properties;
    [HideInInspector] public Vector2Int Size;

    // Properties
    public LuaMapEvent LuaObject { get; private set; }
    public Vector3 TargetPositionPx { get; set; }
    public bool Tracking { get; private set; }
    
    public Vector3 PositionPx {
        get { return transform.localPosition; }
    }
    
    public Map Map {
        get {
            GameObject parentObject = gameObject;
            while (parentObject.transform.parent != null) {
                parentObject = parentObject.transform.parent.gameObject;
                Map map = parentObject.GetComponent<Map>();
                if (map != null) {
                    return map;
                }
            }
            return null;
        }
    }
    
    public ObjectLayer Layer {
        get {
            GameObject parent = gameObject;
            while (parent.transform.parent != null) {
                parent = parent.transform.parent.gameObject;
                ObjectLayer objLayer = parent.GetComponent<ObjectLayer>();
                if (objLayer != null) {
                    return objLayer;
                }
            }
            return null;
        }
    }

    private bool switchEnabled = true;
    public bool SwitchEnabled {
        get { return switchEnabled; }
        set {
            if (value != switchEnabled) {
                GetComponent<Dispatch>().Signal(EventEnabled, value);
            }
            switchEnabled = value;
        }
    }

    // convert from tile coordinates to that spot in world space
    public abstract Vector3 OwnTileToWorld(Vector2Int location);
    public abstract Vector2Int OwnWorldToTile(Vector3 location);

    // if this event were to move in a direction, how would that affect coordinates?
    public abstract Vector2Int OffsetForTiles(OrthoDir dir);

    // perform any pixel-perfect rounding needed for a pixel position
    public abstract Vector3 InternalPositionToDisplayPosition(Vector3 position);

    // what's the direction from this event towards that position?
    public abstract OrthoDir DirectionTo(Vector2Int position);

    // we have a solid TileX/TileY, please move the doll to the correct screen space
    public abstract void SetScreenPositionToMatchTilePosition();
    public abstract void SetTilePositionToMatchScreenPosition();

    // set the one xyz coordinate not controlled by arrow keys
    public abstract void SetDepth();

    // where should is the internal origin of this event?
    public abstract Vector3 GetHandlePosition();

    protected abstract bool UsesSnap();

    protected abstract void DrawGizmoSelf();

    public void Awake() {
        LuaObject = new LuaMapEvent(this);
    }

    public void Start() {
        pixelImperfectPos = PositionPx;

        GenerateLua();

        GetComponent<Dispatch>().RegisterListener(EventCollide, (object payload) => {
            OnCollide((AvatarEvent)payload);
        });
        GetComponent<Dispatch>().RegisterListener(EventInteract, (object payload) => {
            OnInteract((AvatarEvent)payload);
        });
        GetComponent<Dispatch>().RegisterListener(EventEnabled, (object payload) => {
            CheckAutostart((bool)payload);
        });

        var appearanceResult = LuaObject.Evaluate(PropertyAppearance);
        if (appearanceResult.IsNotNil() && GetComponent<CharaEvent>() != null) {
            GetComponent<CharaEvent>().SetAppearanceByTag(appearanceResult.String);
        }

        CheckEnabled();
    }

    public void Update() {
        if (Tracking) {
            var resolution = Map.UnitsPerTile / Map.PxPerTile;
            var x = Mathf.Round(pixelImperfectPos.x * (1.0f / resolution)) * resolution;
            var y = Mathf.Round(pixelImperfectPos.y * (1.0f / resolution)) * resolution;
            transform.position = new Vector3(x, y, transform.position.z);
        }
    }

    public void OnDrawGizmos() {
        if (Selection.activeGameObject == gameObject) {
            Gizmos.color = Color.red;
        } else {
            Gizmos.color = Color.magenta;
        }
        DrawGizmoSelf();
    }

    public void OnValidate() {
        if (Properties == null) {
            Properties = GetComponent<SuperCustomProperties>();
        }
    }

    public void CheckEnabled() {
        SwitchEnabled = !LuaObject.EvaluateBool(PropertyLuaHide, false);
    }

    public bool IsPassable() {
        string passable = GetProperty(PropertyPassable);
        if (passable == "IMPASSABLE") return false;
        if (passable == "PASSABLE") return true;
        if (GetComponent<CharaEvent>() != null) return false;
        return true;
    }

    public bool IsPassableBy(MapEvent other) {
        return IsPassable() || !SwitchEnabled;
    }

    public OrthoDir DirectionTo(MapEvent other) {
        return DirectionTo(other.Position);
    }

    public bool CanPassAt(Vector2Int loc) {
        if (!GetComponent<MapEvent>().SwitchEnabled) {
            return true;
        }
        if (loc.x < 0 || loc.x >= Map.Width || loc.y < 0 || loc.y >= Map.Height) {
            return false;
        }
        foreach (Tilemap layer in Map.layers) {
            if (layer.transform.position.z >= Map.objectLayer.transform.position.z && 
                    !Map.IsChipPassableAt(layer, loc)) {
                return false;
            }
        }
        if (!IsPassable()) {
            foreach (MapEvent mapEvent in Map.GetEventsAt(loc)) {
                if (!mapEvent.IsPassableBy(this)) {
                    return false;
                }
            }
        }

        return true;
    }

    public string GetProperty(string propertyName) {
        if (Properties.TryGetCustomProperty(propertyName, out CustomProperty prop)) {
            return prop.GetValueAsString();
        } else {
            return "";
        }
    }

    public bool ContainsPosition(Vector2Int loc) {
        Vector2Int pos1 = Position;
        Vector2Int pos2 = Position + Size;
        return loc.x >= pos1.x && loc.x < pos2.x && loc.y >= pos1.y && loc.y < pos2.y;
    }

    public void SetPosition(Vector2Int location) {
        Position = location;
        SetScreenPositionToMatchTilePosition();
        SetDepth();
    }

    public void GenerateLua() {
        LuaObject.Set(PropertyLuaHide, GetProperty(PropertyLuaHide));
        LuaObject.Set(PropertyLuaAutostart, GetProperty(PropertyLuaAutostart));
        LuaObject.Set(PropertyLuaCollide, GetProperty(PropertyLuaCollide));
        LuaObject.Set(PropertyLuaInteract, GetProperty(PropertyLuaInteract));
    }

    private void CheckAutostart(bool enabled) {
        LuaContext context = GetComponent<LuaContext>();
        if (enabled && !context.IsRunning()) {
            LuaObject.Run(PropertyLuaAutostart);
        }
    }

    // called when the avatar stumbles into us
    // before the step if impassable, after if passable
    private void OnCollide(AvatarEvent avatar) {
        LuaObject.Run(PropertyLuaCollide);
    }

    // called when the avatar stumbles into us
    // facing us if impassable, on top of us if passable
    private void OnInteract(AvatarEvent avatar) {
        LuaObject.Run(PropertyLuaInteract);
    }

    private LuaScript ParseScript(string lua) {
        if (lua == null || lua.Length == 0) {
            return null;
        } else {
            return new LuaScript(GetComponent<LuaContext>(), lua);
        }
    }

    private LuaCondition ParseCondition(string lua) {
        if (lua == null || lua.Length == 0) {
            return null;
        } else {
           return GetComponent<LuaContext>().CreateCondition(lua);
        }
    }

    public IEnumerator StepRoutine(OrthoDir dir) {
        if (Tracking) {
            yield break;
        }
        
        Position += OffsetForTiles(dir);

        if (GetComponent<CharaEvent>() == null) {
            yield return LinearStepRoutine(dir);
        } else {
            yield return GetComponent<CharaEvent>().StepRoutine(dir);
        }
    }

    public IEnumerator StepMultiRoutine(OrthoDir dir, int count) {
        for (int i = 0; i < count; i += 1) {
            yield return StartCoroutine(StepRoutine(dir));
        }
    }

    public IEnumerator LinearStepRoutine(OrthoDir dir) {
        Tracking = true;
        TargetPositionPx = OwnTileToWorld(Position);
        pixelImperfectPos = PositionPx;
        var tween = DOTween.To(() => pixelImperfectPos, x => pixelImperfectPos = x, TargetPositionPx, 1.0f / TilesPerSecond);
        tween.SetEase(Ease.Linear);
        yield return CoUtils.RunTween(tween);
        transform.position = TargetPositionPx;
        Tracking = false;
    }

    public IEnumerator PathToRoutine(Vector2Int location) {
        List<Vector2Int> path = Map.FindPath(this, location);
        if (path == null) {
            yield break;
        }
        foreach (Vector2Int target in path) {
            OrthoDir dir = DirectionTo(target);
            yield return StartCoroutine(StepRoutine(dir));
        }
    }
}
