using UnityEngine;
using System.Collections;

public class PointerComponent : MonoBehaviour {

    [SerializeField] private PointerLayer pointers = null;

    public void Update() {
        pointers?.SetPointerAt(transform);
    }

    public void OnDisable() {
        pointers?.HidePointer(transform);
    }

    public void Populate(PointerLayer pointers) {
        this.pointers = pointers;
    }
}
