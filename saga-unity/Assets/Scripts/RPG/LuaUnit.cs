using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class LuaUnit {

    public DynValue LuaValue { get; private set; }

    private Unit unit;
    private readonly LuaContext context;

    public LuaUnit(Unit unit, LuaContext context) {
        this.unit = unit;
        this.context = context;
        LuaValue = context.CreateObject();
    }

    // === CALLED BY LUA === 

    public string getSpriteName() {
        return unit.FieldSpriteTag;
    }

    public string getName() {
        return unit.Name;
    }
}
