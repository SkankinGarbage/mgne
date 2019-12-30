using UnityEngine;

public class MapCamera2D : MapCamera {

    public void LateUpdate() {
        ManualUpdate();
    }

    public override void ManualUpdate() {
        base.ManualUpdate();
        Vector3 targetPos = target.transform.position;
        Vector3 oldPos = GetComponent<Camera>().transform.position;
        Vector2Int pixelPos = new Vector2Int(
            (int) (targetPos.x / Map.UnityUnitScale * Map.TileSizePx + Map.TileSizePx / 2),
            (int) (targetPos.y / Map.UnityUnitScale * Map.TileSizePx + Map.TileSizePx / 2));
        Vector3 newPos = new Vector3(
            pixelPos.x / Map.TileSizePx * Map.UnityUnitScale,
            pixelPos.y / Map.TileSizePx * Map.UnityUnitScale, 
            oldPos.z);
        GetComponent<Camera>().transform.position = newPos;
    }
}
