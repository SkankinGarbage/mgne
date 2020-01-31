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
        DestroyChildren();
        foreach (var datum in data) {
            var gameObject = Instantiate(prefab);
            populater(gameObject, datum);
            gameObject.transform.SetParent(transform, worldPositionStays:false);
        }
    }

    private void DestroyChildren() {
        foreach (RectTransform child in transform) {
            Destroy(child.gameObject);
        }
    }
}
