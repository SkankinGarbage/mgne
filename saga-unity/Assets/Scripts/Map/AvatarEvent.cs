using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharaEvent))]
public class AvatarEvent : MonoBehaviour, InputListener, MemoryPopulater {

    public bool WantsToTrack { get; private set; }

    private int pauseCount;
    public bool InputPaused {
        get {
            return pauseCount > 0;
        }
    }

    private CharaEvent chara;
    public CharaEvent Chara {
        get {
            if (chara == null) {
                chara = GetComponent<CharaEvent>();
            }
            return chara;
        }
    }

    private MapEvent parent;
    public MapEvent Parent {
        get {
            if (parent == null) {
                parent = GetComponent<MapEvent>();
            }
            return parent;
        }
    }

    public void Start() {
        Global.Instance().Maps.avatar = this;
        Global.Instance().Input.PushListener(this);
        pauseCount = 0;
    }

    public virtual void Update() {
        WantsToTrack = false;
    }

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) {
        if (Parent.Tracking || InputPaused) {
            return true;
        }
        switch (eventType) {
            case InputManager.Event.Hold:
                switch (command) {
                    case InputManager.Command.Up:
                        TryStep(OrthoDir.North);
                        return false;
                    case InputManager.Command.Down:
                        TryStep(OrthoDir.South);
                        return false;
                    case InputManager.Command.Right:
                        TryStep(OrthoDir.East);
                        return false;
                    case InputManager.Command.Left:
                        TryStep(OrthoDir.West);
                        return false;
                    default:
                        return false;
                }
            case InputManager.Event.Up:
                switch (command) {
                    case InputManager.Command.Confirm:
                        Interact();
                        return false;
                    case InputManager.Command.Cancel:
                        ShowMenu();
                        return false;
                    case InputManager.Command.Debug:
                        Global.Instance().Memory.SaveToSlot(0);
                        return false;
                    default:
                        return false;
                }
            default:
                return false;
        }
    }

    public void PopulateFromMemory(Memory memory) {
        Parent.SetPosition(memory.position);
        Chara.Facing = memory.facing;
    }

    public void PopulateMemory(Memory memory) {
        memory.position = Parent.Position;
        memory.facing = Chara.Facing;
    }

    public void PauseInput() {
        pauseCount += 1;
    }

    public void UnpauseInput() {
        pauseCount -= 1;
    }

    public void SetHidden(bool hidden) {
        Parent.SwitchEnabled = hidden;
    }

    public void OnTeleport() {
        Chara.ResetAnimationTimer();
    }

    private void Interact() {
        Vector2Int target = Parent.Position + Chara.Facing.XY2D();
        List<MapEvent> targetEvents = Parent.Map.GetEventsAt(target);
        foreach (MapEvent tryTarget in targetEvents) {
            if (tryTarget.SwitchEnabled && !tryTarget.IsPassableBy(Parent)) {
                tryTarget.GetComponent<Dispatch>().Signal(MapEvent.EventInteract, this);
                return;
            }
        }

        target = Parent.Position;
        targetEvents = Parent.Map.GetEventsAt(target);
        foreach (MapEvent tryTarget in targetEvents) {
            if (tryTarget.SwitchEnabled && tryTarget.IsPassableBy(Parent)) {
                tryTarget.GetComponent<Dispatch>().Signal(MapEvent.EventInteract, this);
                return;
            }
        }
    }

    private bool TryStep(OrthoDir dir) {
        Vector2Int vectors = Parent.Position;
        Vector2Int vsd = dir.XY2D();
        Vector2Int target = vectors + vsd;
        Chara.Facing = dir;
        List<MapEvent> targetEvents = Parent.Map.GetEventsAt(target);

        List<MapEvent> toCollide = new List<MapEvent>();
        bool passable = Parent.CanPassAt(target);
        foreach (MapEvent targetEvent in targetEvents) {
            toCollide.Add(targetEvent);
            if (!Parent.CanPassAt(target)) {
                passable = false;
            }
        }

        if (passable) {
            WantsToTrack = true;
            StartCoroutine(CoUtils.RunWithCallback(Parent.StepRoutine(dir), () => {
                foreach (MapEvent targetEvent in toCollide) {
                    if (targetEvent.SwitchEnabled) {
                        targetEvent.GetComponent<Dispatch>().Signal(MapEvent.EventCollide, this);
                    }
                }
            }));
        } else {
            foreach (MapEvent targetEvent in toCollide) {
                if (targetEvent.SwitchEnabled && !targetEvent.IsPassableBy(Parent)) {
                    targetEvent.GetComponent<Dispatch>().Signal(MapEvent.EventCollide, this);
                }
            }
        }
        
        return true;
    }

    private void ShowMenu() {
        // oh shiii
    }
}
