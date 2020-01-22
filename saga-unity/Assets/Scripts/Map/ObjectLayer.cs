using UnityEngine;

public class ObjectLayer : MonoBehaviour {

    public Map parent {
        get {
            return GetComponentInParent<Map>();
        }
    }
}
