using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class LuaUnit {

    public DynValue luaValue { get; private set; }

    private Unit unit;
    private LuaContext context;

    public LuaUnit(Unit unit, LuaContext context) {
        this.unit = unit;
        this.context = context;
        luaValue = context.CreateObject();
    }

    // === CALLED BY LUA === 

    public string getSpriteName() {
        return unit.FieldSpriteTag;
    }
}
