using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class MapManager : MonoBehaviour {

    public const string DefaultTransitionTag = "default";
    public const string EventTeleport = "teleport";

    public Map activeMap { get; set; }
    public AvatarEvent avatar { get; set; }

    private MapCamera _camera;
    public new MapCamera camera {
        get {
            if (_camera == null) {
                _camera = FindObjectOfType<MapCamera>();
            }
            return _camera;
        }
        set {
            _camera = value;
        }
    }

    public void Start() {
        activeMap = FindObjectOfType<Map>();
        avatar = activeMap.GetComponentInChildren<AvatarEvent>();
    }

    public void SetUpInitialMap(string mapName) {
        activeMap = InstantiateMap(mapName);
        AddInitialAvatar();
    }

    public IEnumerator TeleportRoutine(string mapName, Vector2Int location, OrthoDir? facing = null, bool isRaw = false) {
        avatar.PauseInput();
        TransitionData data = Global.Instance().Database.Transitions.GetData(DefaultTransitionTag);
        if (!isRaw) {
            yield return camera.GetComponent<FadeComponent>().TransitionRoutine(data, () => {
                RawTeleport(mapName, location, facing);
            });
        } else {
            RawTeleport(mapName, location, facing);
        }
        avatar.UnpauseInput();
    }

    public IEnumerator TeleportRoutine(string mapName, string targetEventName, OrthoDir? facing = null, bool isRaw = false) {
        avatar.PauseInput();
        TransitionData data = Global.Instance().Database.Transitions.GetData(DefaultTransitionTag);
        if (!isRaw) {
            yield return camera.GetComponent<FadeComponent>().TransitionRoutine(data, () => {
                RawTeleport(mapName, targetEventName, facing);
            });
        } else {
            RawTeleport(mapName, targetEventName, facing);
        }
        avatar.UnpauseInput();
    }
    
    private void RawTeleport(string mapName, Vector2Int location, OrthoDir? facing = null) {
        Map newMapInstance = InstantiateMap(mapName);
        RawTeleport(newMapInstance, location, facing);
    }

    private void RawTeleport(string mapName, string targetEventName, OrthoDir? facing = null) {
        Map newMapInstance = InstantiateMap(mapName);
        MapEvent target = newMapInstance.GetEventNamed(targetEventName);
        RawTeleport(newMapInstance, target.Position, facing);
    }

    private void RawTeleport(Map map, Vector2Int location, OrthoDir? facing = null) {
        Assert.IsNotNull(activeMap);
        Assert.IsNotNull(avatar);

        avatar.transform.SetParent(map.objectLayer.transform, false);

        activeMap.OnTeleportAway();
        Destroy(activeMap.gameObject);
        activeMap = map;
        activeMap.OnTeleportTo();
        Global.Instance().Dispatch.Signal(EventTeleport, activeMap);
        avatar.GetComponent<MapEvent>().SetPosition(location);
        avatar.OnTeleport();
        if (facing != null) {
            avatar.Chara.Facing = facing.GetValueOrDefault(OrthoDir.North);
        }
    }

    private Map InstantiateMap(string mapName) {
        if (mapName.EndsWith(".tmx")) {
            mapName = mapName.Substring(0, mapName.IndexOf('.'));
        }
        GameObject newMapObject = null;
        if (activeMap != null) {
            string localPath = Map.ResourcePath + mapName;
            newMapObject = Resources.Load<GameObject>(localPath);
        }
        if (newMapObject == null) {
            newMapObject = Resources.Load<GameObject>(mapName);
        }
        Assert.IsNotNull(newMapObject, "Couldn't find map " + mapName);
        return Instantiate(newMapObject).GetComponent<Map>();
    }

    private void AddInitialAvatar(Memory memory = null) {
        // TODO: 
        avatar = Instantiate(Resources.Load<GameObject>("Prefabs/Avatar")).GetComponent<AvatarEvent>();
        avatar.transform.parent = activeMap.objectLayer.transform;
        activeMap.OnTeleportTo();
        camera.target = avatar.Parent;
        camera.ManualUpdate();
    }
}
