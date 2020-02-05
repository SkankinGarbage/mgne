using UnityEngine;
using System.Collections;

public class PointerComponent : MonoBehaviour {

    [SerializeField] private PointerLayer pointers = null;

    public void Update() {
        pointers?.SetPointerAt(gameObject);
    }

    public void OnDisable() {
        pointers?.HidePointer(gameObject);
    }

    public void Populate(PointerLayer pointers) {
        this.pointers = pointers;
    }
}
