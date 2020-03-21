﻿using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EventAppearance : MonoBehaviour {

    public MapEvent ParentEvent;

    public void Start() {
        Parent().GetComponent<Dispatch>().RegisterListener(MapEvent.EventEnabled, (object payload) => {
            bool enabled = (bool)payload;
            GetComponent<SpriteRenderer>().enabled = enabled;
        });
    }

    public void SetAppearance(string spriteName) {
        Sprite sprite = null;
        if (Application.isEditor) {
#if UNITY_EDITOR
            sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Sprites/" + spriteName + ".png");
#endif
        } else {
            sprite = Resources.Load<Sprite>("Sprites/" + spriteName);
        }
        
        SetAppearance(sprite);
    }

    public void SetAppearance(Sprite sprite) {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    private GameObject Parent() {
        return ParentEvent == null ? transform.parent.gameObject : ParentEvent.gameObject;
    }
}
