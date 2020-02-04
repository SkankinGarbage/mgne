using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointerLayer : MonoBehaviour {

    private const string PrefabPath = "Prefabs/UI/Common/Pointer";

    private GameObject pointer;

    public void SetPointerAt(Transform position) {
        if (pointer == null) {
            pointer = Instantiate(Resources.Load<GameObject>(PrefabPath));
            pointer.transform.SetParent(transform, true);
        }
        pointer.SetActive(true);
        pointer.transform.position = position.position;
        pointer.transform.localScale = new Vector3(1, 1, 1);
    }

    public void HidePointer(Transform at) {
        if (pointer != null && pointer.transform == at) {
            pointer?.SetActive(false);
        }
    }
}
