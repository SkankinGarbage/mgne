﻿using UnityEngine;
using System.Collections;
using MoonSharp.Interpreter;
using Coroutine = MoonSharp.Interpreter.Coroutine;
using System;
using System.Collections.Generic;

/// <summary>
///  A wrapper around Script that represents an environment where a script can execute.
/// </summary>
public class LuaContext {
    
    private const string DefinesPath = "Lua/Defines/GlobalDefines";
    private const string ScenesPath = "Lua/Scenes";

    private static string defines;

    private Script _lua;
    public Script lua {
        get {
            if (_lua == null) {
                _lua = new Script();
            }
            return _lua;
        }
    }

    private Stack<LuaScript> activeScripts = new Stack<LuaScript>();
    private bool forceKilled;

    public virtual void Initialize() {
        LoadDefines(DefinesPath);
        AssignGlobals();
    }

    // make sure the luaobject has been registered via [MoonSharpUserData]
    public void SetGlobal(string key, object luaObject) {
        lua.Globals[key] = luaObject;
    }

    public bool IsRunning() {
        return activeScripts.Count == 0;
    }

    public DynValue CreateObject() {
        return lua.DoString("return {}");
    }

    public DynValue Marshal(object toMarshal) {
        // touch globals to make sure assembly registered
        Global.Instance();

        return DynValue.FromObject(lua, toMarshal);
    }

    public Coroutine CreateScript(string fullScript) {
        try {
            DynValue scriptFunction = lua.DoString(fullScript);
            return lua.CreateCoroutine(scriptFunction).Coroutine;
        } catch (SyntaxErrorException e) {
            Debug.LogError("bad script: " + fullScript + "\n\nerror:\n" + e.DecoratedMessage);
            throw e;
        }
    }

    // all coroutines that are meant to block execution of the script should go through here
    public virtual void RunRoutineFromLua(IEnumerator routine) {
        if (forceKilled) {
            // leave the old instance infinitely suspended
            return;
        }
        Global.Instance().StartCoroutine(CoUtils.RunWithCallback(routine, () => {
            if (activeScripts.Count > 0 && !forceKilled) {
                ResumeAwaitedScript();
            }
        }));
    }

    // meant to be evaluated synchronously
    public LuaCondition CreateCondition(string luaScript) {
        return new LuaCondition(this, lua.LoadString(luaScript));
    }

    // evaluates a lua function in the global context
    public DynValue Evaluate(DynValue function) {
        return lua.Call(function);
    }

    // hang on to a chunk of lua to run later
    public DynValue Load(string luaChunk) {
        return lua.LoadString(luaChunk);
    }

    // kills the current script, useful for debug only
    public void ForceTerminate() {
        forceKilled = true;
    }

    public IEnumerator RunRoutine(string luaString) {
        LuaScript script = new LuaScript(this, luaString);
        yield return RunRoutine(script);
    }

    public virtual IEnumerator RunRoutine(LuaScript script) {
        activeScripts.Push(script);
        forceKilled = false;
        try {
            script.scriptRoutine.Resume();
        } catch (Exception) {
            Debug.Log("Exception during script: " + script + "\n context: " + this);
            throw;
        }
        while (script.scriptRoutine.State != CoroutineState.Dead && !forceKilled) {
            yield return null;
        }
        activeScripts.Pop();
    }

    public IEnumerator RunRoutineFromFile(string filename) {
        if (filename.Contains(".")) {
            filename = filename.Substring(0, filename.IndexOf('.'));
        }
        var asset = Resources.Load<LuaSerializedScript>("Lua/" + filename);
        yield return RunRoutine(asset.luaString);
    }

    protected void ResumeAwaitedScript() {
        activeScripts.Peek().scriptRoutine.Resume();
    }

    protected virtual void AssignGlobals() {
        lua.Globals["debugLog"] = (Action<DynValue>)DebugLog;
        lua.Globals["playSFX"] = (Action<DynValue>)PlaySFX;
        lua.Globals["cs_wait"] = (Action<DynValue>)Wait;
        lua.Globals["cs_play"] = (Action<DynValue>)Play;
        lua.Globals["getSwitch"] = (Func<DynValue, DynValue>)GetSwitch;
        lua.Globals["setSwitch"] = (Action<DynValue, DynValue>)SetSwitch;
        lua.Globals["eventNamed"] = (Func<DynValue, LuaMapEvent>)EventNamed;
        lua.Globals["getHero"] = (Func<DynValue, DynValue>)GetHero;
        lua.Globals["hasItem"] = (Func<DynValue, DynValue>)HasItem;
    }

    protected void LoadDefines(string path) {
        LuaSerializedScript script = Resources.Load<LuaSerializedScript>(path);
        lua.DoString(script.luaString);
    }

    // === LUA CALLABLE ============================================================================

    protected DynValue HasItem(DynValue itemLua) {
        var key = itemLua.String;
        var data = IndexDatabase.Instance().CombatItems.GetData(key);
        return Marshal(Global.Instance().Data.Inventory.ContainsItemType(data));
    }

    protected LuaMapEvent EventNamed(DynValue eventName) {
        MapEvent mapEvent = Global.Instance().Maps.ActiveMap.GetEventNamed(eventName.String);
        if (mapEvent == null) {
            return null;
        } else {
            return mapEvent.LuaObject;
        }
    }

    protected DynValue GetSwitch(DynValue switchName) {
        bool value = Global.Instance().Data.GetSwitch(switchName.String);
        return Marshal(value);
    }

    protected void SetSwitch(DynValue switchName, DynValue value) {
        Global.Instance().Data.SetSwitch(switchName.String, value.Boolean);
    }

    protected void DebugLog(DynValue message) {
        Debug.Log(message.CastToString());
    }

    protected void Wait(DynValue seconds) {
        RunRoutineFromLua(CoUtils.Wait((float)seconds.Number));
    }

    protected void PlaySFX(DynValue sfxKey) {
        Global.Instance().Audio.PlaySFX(sfxKey.String);
    }

    protected void Play(DynValue filename) {
        RunRoutineFromLua(RunRoutineFromFile(filename.String));
    }

    protected DynValue GetHero(DynValue slot) {
        var obj = Global.Instance().Party[(int)slot.Number].GetLuaUnit(this);
        return UserData.Create(obj);
    }
}
