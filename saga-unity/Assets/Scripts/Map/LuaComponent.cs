using UnityEngine;

public class LuaComponent : MonoBehaviour {

    public LuaContext Context { get; private set; } = new LuaCutsceneContext();

    public void Awake() {
        Context.Initialize();
    }
}
