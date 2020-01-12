using UnityEngine;

public class MapCamera2D : MapCamera {

    public void LateUpdate() {
        ManualUpdate();
    }

    public override void ManualUpdate() {
        base.ManualUpdate();

        // assume the target is moving pixel-perfect (w/e)
        Vector3 targetPos = new Vector3(target.transform.position.x, target.transform.position.y, Camera.transform.position.z);
        Camera.transform.position = targetPos;
    }
}
