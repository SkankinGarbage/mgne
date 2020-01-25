using UnityEngine;
using System.Collections;

public class GameDataManager : MonoBehaviour {

    public GameData Data { get; private set; }

    public void Start() {
        Global.Instance().Dispatch.RegisterListener(MapManager.EventTeleport, OnTeleport);
        Data = new GameData();
    }

    public void OnDestroy() {
        Global.Instance()?.Dispatch?.UnregisterListener(MapManager.EventTeleport, OnTeleport);
    }

    private void OnTeleport(object payload) {
        Data.OnTeleportTo((Map)payload);
    }
}
