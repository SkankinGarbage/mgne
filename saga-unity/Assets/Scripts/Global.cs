using UnityEngine;

public class Global : MonoBehaviour {

    private static Global instance;
    
    public InputManager Input { get; private set; }
    public MapManager Maps { get; private set; }
    public AudioManager Audio { get; private set; }
    public SerializationManager Serialization { get; private set; }
    public Dispatch Dispatch { get; private set; }

    public GameData Data => Serialization.Data;
    public SystemData System => Serialization.System;
    public Party Party => Data.Party;

    private UIAnchorPoint ui;
    public UIAnchorPoint UI {
        get {
            if (ui == null) {
                ui = FindObjectOfType<UIAnchorPoint>();
            }
            return ui;
        }
    }

    public static Global Instance() {
        if (instance == null) {
            GameObject globalObject = new GameObject("Globals");
            instance = globalObject.AddComponent<Global>();
            instance.InstantiateManagers();
        }

        return instance;
    }

    public void Awake() {
        DontDestroyOnLoad(gameObject);
        MoonSharp.Interpreter.UserData.RegisterAssembly();

        Serialization.System.SettingFullScreen.OnModify += () => {
            SetFullscreenMode();
        };
    }

    private void InstantiateManagers() {
        gameObject.AddComponent<LuaCutsceneContext>();

        Dispatch = gameObject.AddComponent<Dispatch>();
        Input = gameObject.AddComponent<InputManager>();
        Maps = gameObject.AddComponent<MapManager>();
        Audio = gameObject.AddComponent<AudioManager>();
        Serialization = gameObject.AddComponent<SerializationManager>();

        Serialization.InitializeData();
    }

    private void SetFullscreenMode() {
        Screen.fullScreen = Serialization.System.SettingFullScreen.Value;
    }
}
