using UnityEngine;

[RequireComponent(typeof(FadeComponent))]
public class MapCamera : MonoBehaviour {
    
    public MapEvent target;

    // these are read by sprites, not actually enforced by the cameras
    public bool billboardX;
    public bool billboardY;

    public void OnEnable() {
        Global.Instance().Maps.camera = this;
    }

    public virtual void ManualUpdate() {

    }

    private Camera _camera;
    public virtual Camera Camera {
        get {
            if (_camera == null) {
                _camera = GetComponent<Camera>();
            }
            return _camera;
        }
    }
}
