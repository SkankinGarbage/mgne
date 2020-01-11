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

    public const string EventMove = "move";
    public const string EventInteract = "interact";
    public const string EventCollide = "collide";
    public const string EventEnabled = "enabled";
    
    public const string PropertyLuaHide = "hide";
    public const string PropertyAppearance = "appearance";
    public const string PropertyPassable = "passable";

    public const string PropertyLuaCollide = "onCollide";
    public const string PropertyLuaInteract = "onInteract";
    public const string PropertyLuaAutostart = "onEnter";

    protected const int TilesPerSecond = 8;

    // Editor properties
    [HideInInspector] public Vector2Int position = new Vector2Int(0, 0);
    [HideInInspector] public SuperCustomProperties properties;
    [HideInInspector] public Vector2Int size;

    // Properties
    public LuaMapEvent luaObject { get; private set; }
    public Vector3 targetPositionPx { get; set; }
    public bool tracking { get; private set; }
    
    public Vector3 positionPx {
        get { return transform.localPosition; }
    }
    
    public Map parent {
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
    
    public ObjectLayer layer {
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

    private bool _switchEnabled = true;
    public bool switchEnabled {
        get { return _switchEnabled; }
        set {
            if (value != _switchEnabled) {
                GetComponent<Dispatch>().Signal(EventEnabled, value);
            }
            _switchEnabled = value;
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
        luaObject = new LuaMapEvent(this);
    }

    public void Start() {
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

        var appearanceResult = luaObject.Evaluate(PropertyAppearance);
        if (appearanceResult.IsNotNil() && GetComponent<CharaEvent>() != null) {
            GetComponent<CharaEvent>().SetAppearanceByTag(appearanceResult.String);
        }

        CheckEnabled();
    }

    public void OnDrawGizmos() {
        if (Selection.activeGameObject == gameObject) {
            Gizmos.color = Color.red;
        } else {
            Gizmos.color = Color.magenta;
        }
        DrawGizmoSelf();
    }

    public void CheckEnabled() {
        switchEnabled = !luaObject.EvaluateBool(PropertyLuaHide, false);
    }

    public bool IsPassable() {
        string passable = GetProperty(PropertyPassable);
        if (passable == "IMPASSABLE") return false;
        if (passable == "PASSABLE") return true;
        if (GetProperty(PropertyAppearance) != null) return false;
        return true;
    }

    public bool IsPassableBy(MapEvent other) {
        return IsPassable() || !switchEnabled;
    }

    public OrthoDir DirectionTo(MapEvent other) {
        return DirectionTo(other.position);
    }

    public bool CanPassAt(Vector2Int loc) {
        if (!GetComponent<MapEvent>().switchEnabled) {
            return true;
        }
        if (loc.x < 0 || loc.x >= parent.width || loc.y < 0 || loc.y >= parent.height) {
            return false;
        }
        foreach (Tilemap layer in parent.layers) {
            if (layer.transform.position.z >= parent.objectLayer.transform.position.z && 
                    !parent.IsChipPassableAt(layer, loc)) {
                return false;
            }
        }
        if (!IsPassable()) {
            foreach (MapEvent mapEvent in parent.GetEventsAt(loc)) {
                if (!mapEvent.IsPassableBy(this)) {
                    return false;
                }
            }
        }

        return true;
    }

    public string GetProperty(string propertyName) {
        if (properties.TryGetCustomProperty(propertyName, out CustomProperty prop)) {
            return prop.GetValueAsString();
        } else {
            return "";
        }
    }

    public IEnumerator PathToRoutine(Vector2Int location) {
        List<Vector2Int> path = parent.FindPath(this, location);
        if (path == null) {
            yield break;
        }
        MapEvent mapEvent = GetComponent<MapEvent>();
        foreach (Vector2Int target in path) {
            OrthoDir dir = mapEvent.DirectionTo(target);
            yield return StartCoroutine(GetComponent<MapEvent>().StepRoutine(dir));
        }
    }

    public bool ContainsPosition(Vector2Int loc) {
        Vector2Int pos1 = position;
        Vector2Int pos2 = position + size;
        return loc.x >= pos1.x && loc.x < pos2.x && loc.y >= pos1.y && loc.y < pos2.y;
    }

    public void SetPosition(Vector2Int location) {
        position = location;
        SetScreenPositionToMatchTilePosition();
        SetDepth();
    }

    public void GenerateLua() {
        luaObject.Set(PropertyLuaHide, GetProperty(PropertyLuaHide));
        luaObject.Set(PropertyLuaAutostart, GetProperty(PropertyLuaAutostart));
        luaObject.Set(PropertyLuaCollide, GetProperty(PropertyLuaCollide));
        luaObject.Set(PropertyLuaInteract, GetProperty(PropertyLuaInteract));
    }

    private void CheckAutostart(bool enabled) {
        LuaContext context = GetComponent<LuaContext>();
        if (enabled && !context.IsRunning()) {
            luaObject.Run(PropertyLuaAutostart);
        }
    }

    // called when the avatar stumbles into us
    // before the step if impassable, after if passable
    private void OnCollide(AvatarEvent avatar) {
        luaObject.Run(PropertyLuaCollide);
    }

    // called when the avatar stumbles into us
    // facing us if impassable, on top of us if passable
    private void OnInteract(AvatarEvent avatar) {
        luaObject.Run(PropertyLuaInteract);
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
        if (tracking) {
            yield break;
        }
        
        position += OffsetForTiles(dir);

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
        tracking = true;
        targetPositionPx = OwnTileToWorld(position);
        //if (TilesPerSecond == 0) {
        //    transform.localPosition = targetPositionPx;
        //} else {
            var tween = transform.DOLocalMove(targetPositionPx, 1.0f / TilesPerSecond, UsesSnap());
            tween.SetEase(Ease.Linear);
            yield return CoUtils.RunTween(tween);
        //}
        tracking = false;
    }
}
