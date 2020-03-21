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

    public override void Initialize() {
        base.Initialize();
        LoadDefines(DefinesPath);
    }

    public override void RunRoutineFromLua(IEnumerator routine) {
        if (MapOverlayUI.Instance().textbox.isDisplaying) {
            MapOverlayUI.Instance().textbox.MarkHiding();
            base.RunRoutineFromLua(CoUtils.RunSequence(new IEnumerator[] {
                MapOverlayUI.Instance().textbox.DisableRoutine(),
                routine,
            }));
        } else {
            base.RunRoutineFromLua(routine);
        }
    }

    public void RunTextboxRoutineFromLua(IEnumerator routine) {
        base.RunRoutineFromLua(routine);
    }

    protected void ResumeNextFrame() {
        Global.Instance().StartCoroutine(ResumeRoutine());
    }
    protected IEnumerator ResumeRoutine() {
        yield return null;
        ResumeAwaitedScript();
    }

    protected override void AssignGlobals() {
        base.AssignGlobals();
        lua.Globals["playBGM"] = (Action<DynValue>)PlayBGM;
        lua.Globals["playSound"] = (Action<DynValue>)PlaySound;
        lua.Globals["sceneSwitch"] = (Action<DynValue, DynValue>)SetSwitch;
        lua.Globals["face"] = (Action<DynValue, DynValue>)Face;
        lua.Globals["hideHero"] = (Action<DynValue>)HideHero;
        lua.Globals["addMember"] = (Action<DynValue>)AddMember;
        lua.Globals["removeMember"] = (Action)RemoveMember;
        lua.Globals["addItem"] = (Action<DynValue, DynValue>)AddItem;
        lua.Globals["addCollectable"] = (Action<DynValue>)AddCollectable;
        lua.Globals["removeItem"] = (Action<DynValue, DynValue>)RemoveItem;
        lua.Globals["cs_teleport"] = (Action<DynValue, DynValue, DynValue, DynValue, DynValue>)Teleport;
        lua.Globals["cs_targetTele"] = (Action<DynValue, DynValue, DynValue, DynValue>)TargetTeleport;
        lua.Globals["cs_fadeOutBGM"] = (Action<DynValue>)FadeOutBGM;
        lua.Globals["cs_fade"] = (Action<DynValue>)Fade;
        lua.Globals["cs_speak"] = (Action<DynValue, DynValue>)Speak;
        lua.Globals["cs_walk"] = (Action<DynValue, DynValue, DynValue, DynValue>)Walk;
        lua.Globals["cs_path"] = (Action<DynValue, DynValue, DynValue, DynValue>)Path;
        lua.Globals["cs_pathEvent"] = (Action<DynValue, DynValue, DynValue>)PathEvent;
        lua.Globals["cs_battle"] = (Action<DynValue, DynValue>)Battle;
        lua.Globals["cs_recruit"] = (Action<DynValue>)Recruit;
        lua.Globals["cs_inn"] = (Action)Inn;
        lua.Globals["cs_shop"] = (Action<DynValue>)Shop;
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
        var speakerString = speaker.IsNil() ? null : speaker.String;
        var textString = text.IsNil() ? null : text.String;
        if (speaker.String.Contains(":")) {
            textString = speakerString.Split(':')[1].Substring(2);
            speakerString = speakerString.Split(':')[0];
        }
        RunTextboxRoutineFromLua(MapOverlayUI.Instance().textbox.SpeakRoutine(speakerString, textString));
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
                ResumeNextFrame();
            }
        } else {
            var function = eventLua.Table.Get("path");
            function.Function.Call(eventLua, targetArg1, targetArg2, targetArg3);
        }
    }

    private void PathEvent(DynValue moverLua, DynValue targetLua, DynValue waitLua) {
        var map = Global.Instance().Maps.ActiveMap;
        var mover = map.GetEventNamed(moverLua.String);
        var target = map.GetEventNamed(targetLua.String);
        var routine = mover.PathToRoutine(target.Position, ignoreEvents:true);
        if (waitLua.IsNil() || waitLua.Boolean) {
            RunRoutineFromLua(routine);
        } else {
            mover.StartCoroutine(routine);
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
        var globals = Global.Instance();
        RunRoutineFromLua(globals.Maps.Camera.GetComponent<FadeComponent>().FadeRoutine(fade, invert));
    }

    private void Battle(DynValue partyLua, DynValue bgmLua) {
        var partyTag = partyLua.String;
        var bgmTag = bgmLua.IsNil() ? null : bgmLua.String;
        Party party = null;
        var partyData = IndexDatabase.Instance().Parties.GetDataOrNull(partyTag);
        if (partyData != null) {
            party = new Party(partyData);
        } else {
            var encounterData = IndexDatabase.Instance().Encounters.GetData(partyTag);
            party = new Party(encounterData);
        }
        RunRoutineFromLua(BattleView.SpawnBattleRoutine(party, bgmTag));
    }

    private void Recruit(DynValue recruitKey) {
        var data = IndexDatabase.Instance().Recruits.GetData(recruitKey.String);
        RunRoutineFromLua(CoUtils.TaskRoutine(RecruitMenu.ShowDefault().DoMenuAsync(data)));
    }

    private void Inn() {
        RunRoutineFromLua(CoUtils.TaskRoutine(InnMenuView.ShowDefault().DoMenuAsync()));
    }

    private void Shop(DynValue shopKey) {
        var data = IndexDatabase.Instance().Shops.GetData(shopKey.String);
        RunRoutineFromLua(CoUtils.TaskRoutine(ShopMenuView.ShowDefault(data).DoMenuAsync()));
    }

    private void AddItem(DynValue itemLua, DynValue isCollectableLua) {
        var key = itemLua.String;
        var isCollectable = !isCollectableLua.IsNil() && isCollectableLua.Boolean;
        if (isCollectable) {
            Global.Instance().Data.Collectables.AddItem(key);
        } else {
            var data = IndexDatabase.Instance().CombatItems.GetData(key);
            var item = new CombatItem(data);
            Global.Instance().Data.Inventory.Add(item);
        }
    }

    private void AddCollectable(DynValue itemLua) {
        Global.Instance().Data.Collectables.AddItem(itemLua.String);
    }

    private void RemoveItem(DynValue itemLua, DynValue isCollectableLua) {
        var key = itemLua.String;
        var isCollectable = isCollectableLua.IsNil() || isCollectableLua.Boolean;
        if (isCollectable) {
            Global.Instance().Data.Collectables.RemoveItem(key);
        } else {
            var data = IndexDatabase.Instance().CombatItems.GetData(key);
            Global.Instance().Data.Inventory.DropItemType(data);
        }
    }

    private void AddMember(DynValue unitLua) {
        var data = IndexDatabase.Instance().Units.GetData(unitLua.String);
        Global.Instance().Party.AddMember(new Unit(data));
    }

    private void RemoveMember() {
        var fifthMember = Global.Instance().Party[4];
        Global.Instance().Party.RemoveMember(fifthMember);
    }
}
