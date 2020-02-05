using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointerLayer : MonoBehaviour {

    private const string PrefabPath = "Prefabs/UI/Common/Pointer";

    private GameObject pointer;
    private GameObject pointerOwner;

    public void SetPointerAt(GameObject originalParent) {
        if (pointer == null) {
            pointer = Instantiate(Resources.Load<GameObject>(PrefabPath));
            pointer.transform.SetParent(transform, true);
        }
        pointer.SetActive(true);
        pointer.transform.position = originalParent.transform.position;
        pointer.transform.localScale = new Vector3(1, 1, 1);

        pointerOwner = originalParent;
    }

    public void HidePointer(GameObject originalParent) {
        if (pointer != null && originalParent == pointerOwner) {
            pointer?.SetActive(false);
        }
    }
}
