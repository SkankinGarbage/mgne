using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All MapEvents own a lua equivalent that has functions stored on it
/// </summary>
[MoonSharpUserData]
public class LuaMapEvent {

    public DynValue luaValue { get; private set; }

    private MapEvent mapEvent;
    private LuaContext context;
    private Dictionary<string, DynValue> values;

    public LuaMapEvent(MapEvent mapEvent) {
        this.mapEvent = mapEvent;
        values = new Dictionary<string, DynValue>();
        context = mapEvent.GetComponent<LuaContext>();
        luaValue = context.Marshal(this);
        context.lua.Globals["this"] = luaValue;
    }

    // meant to be called with the key/value of a lualike property on a Tiled object
    // accepts nil and zero-length as no-ops
    [MoonSharpHidden]
    public void Set(string name, string luaChunk) {
        if (luaChunk != null && luaChunk.Length > 0) {
            values[name] = context.Load(luaChunk);
        }
    }

    [MoonSharpHidden]
    public void Run(string eventName, Action callback = null) {
        if (!values.ContainsKey(eventName)) {
            callback?.Invoke();
        } else {
            LuaScript script = new LuaScript(context, values[eventName]);
            Global.Instance().StartCoroutine(CoUtils.RunWithCallback(script.RunRoutine(), callback));
        }
    }

    [MoonSharpHidden]
    public DynValue Evaluate(string propertyName) {
        if (!values.ContainsKey(propertyName)) {
            return DynValue.Nil;
        } else {
            return context.Evaluate(values[propertyName]);
        }
    }

    [MoonSharpHidden]
    public bool EvaluateBool(string propertyName, bool defaultValue = false) {
        if (!values.ContainsKey(propertyName)) {
            return defaultValue;
        } else {
            DynValue result = Evaluate(propertyName);
            return result.Boolean;
        }
    }

    // === CALLED BY LUA === 

    public void face(string directionName) {
        mapEvent.GetComponent<CharaEvent>().Facing = OrthoDirExtensions.Parse(directionName);
    }

    public void faceToward(LuaMapEvent other) {
        mapEvent.GetComponent<CharaEvent>().Facing = mapEvent.DirectionTo(other.mapEvent);
    }

    public int x() {
        return mapEvent.Position.x;
    }

    public int y() {
        return mapEvent.Position.y;
    }

    public void debuglog() {
        Debug.Log("Debug: " + mapEvent.name);
    }

    public void path(int x, int y, bool wait = false) {
        var routine = mapEvent.PathToRoutine(new Vector2Int(x, y));
        if (wait) {
            context.RunRoutineFromLua(routine);
        } else {
            mapEvent.StartCoroutine(routine);
        }
    }

    public void walk(string directionName, int count) {
       context.RunRoutineFromLua(mapEvent.StepMultiRoutine(OrthoDirExtensions.Parse(directionName), count));
    }

    public void wander() {
        var offset = UnityEngine.Random.Range(0, 4);
        for (var @base = 0; @base < 4; @base += 1) {
            var ordinal = (offset + @base) % 4;
            var dir = (OrthoDir)ordinal;
            var newPos = mapEvent.Position + dir.XY2D();
            if (mapEvent.Map.IsChipPassableAt(newPos)) {
                if (Global.Instance().Maps.Avatar.Parent.Position == newPos) {
                    mapEvent.GetComponent<CharaEvent>().Facing = dir;
                    Run(MapEvent.PropertyLuaCollide);
                    break;
                } else if (mapEvent.CanPassAt(newPos) && mapEvent.Map.GetEventAt<MapEvent>(newPos) == null) {
                    context.RunRoutineFromLua(mapEvent.StepRoutine(dir));
                    break;
                }
            }
        }
    }

    public void cs_step(string directionName) {
        context.RunRoutineFromLua(mapEvent.StepRoutine(OrthoDirExtensions.Parse(directionName)));
    }
}
