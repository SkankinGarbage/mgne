using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharaEvent))]
public class AvatarEvent : MonoBehaviour, IInputListener {

    public bool WantsToTrack { get; private set; }

    private int pauseCount;
    public bool IsInputPaused {
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

    public Map Map => Parent.Map;

    public void Start() {
        Global.Instance().Maps.Avatar = this;
        Global.Instance().Input.PushListener(this);
        pauseCount = 0;
    }

    public virtual void Update() {
        WantsToTrack = false;
    }

    public void UpdateAppearance() {
        Chara.SetAppearanceByTag(Global.Instance().Party.Leader.FieldSpriteTag);
    }

    public bool OnCommand(InputManager.Command command, InputManager.Event eventType) {
        if (Parent.Tracking || IsInputPaused) {
            return false;
        }
        switch (eventType) {
            case InputManager.Event.Hold:
                switch (command) {
                    case InputManager.Command.Up:
                        TryStep(OrthoDir.North);
                        return true;
                    case InputManager.Command.Down:
                        TryStep(OrthoDir.South);
                        return true;
                    case InputManager.Command.Right:
                        TryStep(OrthoDir.East);
                        return true;
                    case InputManager.Command.Left:
                        TryStep(OrthoDir.West);
                        return true;
                    default:
                        return true;
                }
            case InputManager.Event.Down:
                switch (command) {
                    case InputManager.Command.Confirm:
                        Interact();
                        return true;
                    case InputManager.Command.Menu:
                    case InputManager.Command.Cancel:
                        ShowMenu();
                        return true;
                    case InputManager.Command.Debug:
                        Global.Instance().Serialization.SaveToSlot(0);
                        return true;
                    default:
                        return true;
                }
            default:
                return true;
        }
    }

    public void PauseInput() {
        pauseCount += 1;
        WantsToTrack = false;
    }

    public void UnpauseInput() {
        if (pauseCount > 0) {
            pauseCount -= 1;
        }
    }

    public void SetHidden(bool hidden) {
        Global.Instance().Data.SetSwitch("hide_hero", hidden);
    }

    public void OnTeleport() {
        Chara.ResetAnimationTimer();
    }

    private void Interact() {
        var offset = Chara.Facing.XY2D();
        var target = Parent.Position + offset;
        var targetEvents = Parent.Map.GetEventsAt(target);
        if (Parent.Map.HasTilePropertyAt(target, tile => tile != null ? tile.IsCounter : false)) {
            target += offset;
            targetEvents.AddRange(Parent.Map.GetEventsAt(target));
        }
        foreach (MapEvent tryTarget in targetEvents) {
            if (tryTarget.IsSwitchEnabled && !tryTarget.IsPassableBy(Parent)) {
                tryTarget.GetComponent<Dispatch>().Signal(MapEvent.EventInteract, this);
                return;
            }
        }

        target = Parent.Position;
        targetEvents = Parent.Map.GetEventsAt(target);
        foreach (MapEvent tryTarget in targetEvents) {
            if (tryTarget.IsSwitchEnabled && tryTarget.IsPassableBy(Parent)) {
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
            StartCoroutine(CoUtils.RunParallel(new IEnumerator[] {
                CoUtils.RunWithCallback(Parent.StepRoutine(dir), () => {
                    foreach (var targetEvent in toCollide) {
                        if (targetEvent.IsSwitchEnabled) {
                            targetEvent.GetComponent<Dispatch>().Signal(MapEvent.EventCollide, this);
                        }
                    }
                    foreach (var targetEvent in Parent.Map.GetEvents<MapEvent>()) {
                        if (targetEvent.IsSwitchEnabled) {
                            targetEvent.GetComponent<Dispatch>().Signal(MapEvent.EventStep, this);
                        }
                    }
                    Global.Instance().Maps.ActiveMap.OnStepEnded();
                }),
                OnStepStartRoutine(),
            }, this));
        } else {
            foreach (var targetEvent in toCollide) {
                if (targetEvent.IsSwitchEnabled && !targetEvent.IsPassableBy(Parent)) {
                    targetEvent.GetComponent<Dispatch>().Signal(MapEvent.EventCollide, this);
                }
            }
        }
        
        return true;
    }

    private IEnumerator OnStepStartRoutine() {
        Global.Instance().Maps.ActiveMap.OnStepStarted();
        yield return null;
    }

    private void ShowMenu() {
        MainMenuView.ShowDefault();
    }
}
