using UnityEngine;
using System.Collections;
using System;
using MoonSharp.Interpreter;

public class LuaCutsceneContext : LuaContext {

    private static readonly string DefinesPath = "Lua/Defines/CutsceneDefines";

    public override IEnumerator RunRoutine(LuaScript script) {
        if (Global.Instance().Maps.Avatar != null) {
            Global.Instance().Maps.Avatar.PauseInput();
        }
        yield return base.RunRoutine(script);
        if (MapOverlayUI.Instance().textbox.isDisplaying) {
            yield return MapOverlayUI.Instance().textbox.DisableRoutine();
        }
        if (Global.Instance().Maps.Avatar != null) {
            Global.Instance().Maps.Avatar.UnpauseInput();
        }
    }

    public void Start() {
        //lua.Globals["avatar"] = Global.Instance().Maps.Avatar.GetComponent<MapEvent>().luaObject;
    }

    public override void Awake() {
        base.Awake();
        LoadDefines(DefinesPath);
    }

    public override void RunRoutineFromLua(IEnumerator routine) {
        if (MapOverlayUI.Instance().textbox.isDisplaying) {
            routine = CoUtils.RunSequence(new IEnumerator[] {
                MapOverlayUI.Instance().textbox.DisableRoutine(),
                routine,
            });
        }

        base.RunRoutineFromLua(routine);
    }

    public void RunTextboxRoutineFromLua(IEnumerator routine) {
        base.RunRoutineFromLua(routine);
    }

    protected override void AssignGlobals() {
        base.AssignGlobals();
        lua.Globals["playBGM"] = (Action<DynValue>)PlayBGM;
        lua.Globals["playSound"] = (Action<DynValue>)PlaySound;
        lua.Globals["sceneSwitch"] = (Action<DynValue, DynValue>)SetSwitch;
        lua.Globals["face"] = (Action<DynValue, DynValue>)Face;
        lua.Globals["hideHero"] = (Action<DynValue>)PlayBGM;
        lua.Globals["cs_teleport"] = (Action<DynValue, DynValue, DynValue, DynValue, DynValue>)Teleport;
        lua.Globals["cs_targetTele"] = (Action<DynValue, DynValue, DynValue, DynValue>)TargetTeleport;
        lua.Globals["cs_fadeOutBGM"] = (Action<DynValue>)FadeOutBGM;
        lua.Globals["cs_speak"] = (Action<DynValue, DynValue>)Speak;
        lua.Globals["cs_walk"] = (Action<DynValue, DynValue, DynValue, DynValue>)Walk;
        lua.Globals["cs_path"] = (Action<DynValue, DynValue, DynValue, DynValue>)Path;
        lua.Globals["fade"] = (Action<DynValue>)Fade;
        lua.Globals["cs_battle"] = (Action<DynValue, DynValue>)Battle;
    }

    // === LUA CALLABLE ============================================================================

    private void PlayBGM(DynValue bgmKey) {
        Global.Instance().Audio.PlayBGM(bgmKey.String);
    }

    private void PlaySound(DynValue soundKey) {
        Global.Instance().Audio.PlaySFX(soundKey.String);
    }

    private void Teleport(DynValue mapName, DynValue x, DynValue y, DynValue facingLua, DynValue rawLua) {
        OrthoDir? facing = null;
        if (!facingLua.IsNil()) facing = OrthoDirExtensions.Parse(facingLua.String);
        var loc = new Vector2Int((int)x.Number, (int)y.Number);
        var raw = rawLua.IsNil() ? false : rawLua.Boolean;
        RunRoutineFromLua(Global.Instance().Maps.TeleportRoutine(mapName.String, loc, facing, raw));
    }

    private void TargetTeleport(DynValue mapName, DynValue targetEventName, DynValue facingLua, DynValue rawLua) {
        OrthoDir? facing = null;
        if (!facingLua.IsNil()) facing = OrthoDirExtensions.Parse(facingLua.String);
        var raw = rawLua.IsNil() ? false : rawLua.Boolean;
        RunRoutineFromLua(Global.Instance().Maps.TeleportRoutine(mapName.String, targetEventName.String, facing, raw));
    }

    private void FadeOutBGM(DynValue seconds) {
        RunRoutineFromLua(Global.Instance().Audio.FadeOutRoutine((float)seconds.Number));
    }

    private void Speak(DynValue speaker, DynValue text) {
        RunTextboxRoutineFromLua(MapOverlayUI.Instance().textbox.SpeakRoutine(speaker.String, text.IsNil() ? null : text.String));
    }

    private void Walk(DynValue eventLua, DynValue steps, DynValue directionLua, DynValue waitLua) {
        if (eventLua.Type == DataType.String) {
            var @event = Global.Instance().Maps.ActiveMap.GetEventNamed(eventLua.String);
            if (@event == null) {
                Debug.LogError("Couldn't find event " + eventLua.String);
            } else {
                var routine = @event.StepMultiRoutine(OrthoDirExtensions.Parse(directionLua.String), (int)steps.Number);
                if (!waitLua.IsNil() && waitLua.Boolean) {
                    RunRoutineFromLua(routine);
                } else {
                    @event.StartCoroutine(routine);
                }
            }
        } else {
            var function = eventLua.Table.Get("walk");
            function.Function.Call(steps, directionLua, waitLua);
        }
    }

    private void Path(DynValue eventLua, DynValue targetArg1, DynValue targetArg2, DynValue targetArg3) {
        bool wait;
        Vector2Int target;
        if (targetArg1.Type == DataType.String) {
            var targetEvent = Global.Instance().Maps.ActiveMap.GetEventNamed(targetArg1.String);
            if (targetEvent == null) {
                Debug.LogError("Couldn't find event " + targetArg1.String);
                return;
            }
            target = targetEvent.Position;
            wait = targetArg2.Boolean;
        } else {
            target = new Vector2Int((int)targetArg1.Number, (int)targetArg2.Number);
            wait = targetArg3.Boolean;
        }

        if (eventLua.Type == DataType.String) {
            var @event = Global.Instance().Maps.ActiveMap.GetEventNamed(eventLua.String);
            if (@event == null) {
                Debug.LogError("Couldn't find event " + eventLua.String);
                return;
            }

            var routine = @event.PathToRoutine(target);
            if (wait) {
                RunRoutineFromLua(routine);
            } else {
                @event.StartCoroutine(routine);
            }
        } else {
            var function = eventLua.Table.Get("path");
            function.Function.Call(eventLua, targetArg1, targetArg2, targetArg3);
        }
    }

    private void Face(DynValue eventName, DynValue dir) {
        var @event = Global.Instance().Maps.ActiveMap.GetEventNamed(eventName.String);
        if (@event == null) {
            Debug.LogError("Couldn't find event " + eventName.String);
        } else {
            @event.GetComponent<CharaEvent>().Facing = OrthoDirExtensions.Parse(dir.String);
        }
    }

    private void HideHero(DynValue hidden) {
        Global.Instance().Maps.Avatar.SetHidden(hidden.Boolean);
    }

    private FadeData lastFade;
    private void Fade(DynValue type) {
        var typeString = type.String;
        FadeData fade;
        bool invert = false;
        if (typeString == "normal") {
            fade = lastFade;
            invert = true;
        } else {
            fade = IndexDatabase.Instance().Fades.GetData(typeString);
        }
        lastFade = fade;
        StartCoroutine(Global.Instance().Maps.Camera.GetComponent<FadeComponent>().FadeRoutine(fade, invert));
    }

    private void Battle(DynValue partyLua, DynValue bgmLua) {
        var partyTag = partyLua.String;
        var bgmTag = bgmLua.IsNil() ? null : bgmLua.String;
        var party = IndexDatabase.Instance().Parties.GetData(partyTag);
        RunRoutineFromLua(BattleView.SpawnBattleRoutine(party, bgmTag));
    }
}
