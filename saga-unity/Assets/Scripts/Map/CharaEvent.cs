﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * For our purposes, a CharaEvent is anything that's going to be moving around the map
 * or has a physical appearance. For parallel process or whatevers, they won't have this.
 */
[DisallowMultipleComponent]
[RequireComponent(typeof(FieldSpritesheetComponent))]
public class CharaEvent : MonoBehaviour {
    
    private const float DesaturationDuration = 0.5f;
    private const float StepsPerSecond = 2.0f;

    public Doll Doll;

    [SerializeField] private bool alwaysAnimates = true;
    
    private Vector2 lastPosition;
    private bool wasSteppingLastFrame;
    private Vector3 targetPx;
    private float moveTime;
    private bool stepping;

    public MapEvent Parent { get { return GetComponent<MapEvent>(); } }
    public Map Map { get { return Parent.Map; } }
    public int StepCount => sprites.StepCount;

    private FieldSpritesheetComponent sprites;
    public FieldSpritesheetComponent Sprites {
        get {
            if (sprites == null) {
                sprites = GetComponent<FieldSpritesheetComponent>();
            }
            return sprites;
        }
    }

    [SerializeField] [HideInInspector] private OrthoDir _facing = OrthoDir.South;
    public OrthoDir Facing {
        get { return _facing; }
        set {
            _facing = value;
            UpdateAppearance();
        }
    }

    private List<SpriteRenderer> _renderers;
    protected List<SpriteRenderer> Renderers {
        get {
            if (_renderers == null) {
                _renderers = new List<SpriteRenderer> {
                    Doll.Renderer
                };
            }
            return _renderers;
        }
    }

    public void Start() {
        GetComponent<Dispatch>().RegisterListener(MapEvent.EventEnabled, (object payload) => {
            UpdateEnabled((bool)payload);
        });
        GetComponent<Dispatch>().RegisterListener(MapEvent.EventInteract, (object payload) => {
            Facing = Parent.DirectionTo(Global.Instance().Maps.Avatar.GetComponent<MapEvent>());
        });
        UpdateEnabled(Parent.IsSwitchEnabled);
    }

    public void Update() {
        bool steppingThisFrame = IsSteppingThisFrame();
        stepping = steppingThisFrame || wasSteppingLastFrame;
        if (!steppingThisFrame && !wasSteppingLastFrame) {
            moveTime = 0.0f;
        } else {
            moveTime += Time.deltaTime;
        }
        wasSteppingLastFrame = steppingThisFrame;
        lastPosition = transform.position;

        UpdateAppearance();
    }

    public void UpdateEnabled(bool enabled) {
        foreach (SpriteRenderer renderer in Renderers) {
            renderer.enabled = enabled;
        }
    }

    public void UpdateAppearance() {
        Doll.Renderer.sprite = SpriteForMain();
    }

    public void FaceToward(MapEvent other) {
        Facing = Parent.DirectionTo(other);
    }

    public void ResetAnimationTimer() {
        moveTime = 0.0f;
    }

    public void SetAppearanceByTag(string fieldSpriteTag) {
        Sprites.SetByTag(fieldSpriteTag);
        UpdateAppearance();
    }

    public IEnumerator StepRoutine(OrthoDir dir) {
        Facing = dir;
        Vector2Int offset = Parent.OffsetForTiles(dir);
        Vector3 startPx = Parent.PositionPx;
        targetPx = Parent.OwnTileToWorld(Parent.Position);
        yield return Parent.LinearStepRoutine(dir);
    }

    private bool IsSteppingThisFrame() {
        Vector2 position = transform.position;
        Vector2 delta = position - lastPosition;
        return alwaysAnimates || (delta.sqrMagnitude > 0 && delta.sqrMagnitude < Map.PxPerTile) || Parent.Tracking ||
            (GetComponent<AvatarEvent>() && GetComponent<AvatarEvent>().WantsToTrack);
    }

    private Sprite SpriteForMain() {
        int x = Mathf.FloorToInt(moveTime * StepsPerSecond) % Sprites.StepCount;
        if (x == 3) x = 1;
        if (!stepping) x = 1;
        return sprites.FrameBySlot(x, Facing);
    }
}
