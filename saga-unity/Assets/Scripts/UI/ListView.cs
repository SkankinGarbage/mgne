using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Adds children and populates them
/// </summary>
/// <remarks>
/// Unoptimized. Should work from an object pool in the future.
/// </remarks>
public class ListView : MonoBehaviour {

    [SerializeField] private GameObject prefab = null;

    public void Populate<T>(IEnumerable<T> data, Action<GameObject, T> populater) {
        var index = 0;
        foreach (var datum in data) {
            GameObject gameObject;
            if (transform.childCount > index) {
                gameObject = Instantiate(prefab);
                gameObject.transform.SetParent(transform, worldPositionStays: false);
            } else {
                var child = transform.GetChild(index);
                gameObject = child.gameObject;
            }
            
            populater(gameObject, datum);
            
            index += 1;
        }

        // destroy extra children
        var offset = index;
        for (; index < transform.childCount; index += 1) {
            Destroy(transform.GetChild(index - offset).gameObject);
        }
    }
}
