using Newtonsoft.Json;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SerializationManager : MonoBehaviour {

    public const int SaveSlotCount = 5;

    private const int CurrentSaveVersion = 1;
    private const string SystemMemoryName = "system.sav";
    private const string SaveGameSuffix = ".sav";

    public GameData Data { get; private set; }
    public SystemData SystemData { get; private set; }

    private JsonSerializer serializer;

    public void Awake() {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new StatSetConverter());
        settings.Converters.Add(new UnitConverter());
        serializer = JsonSerializer.Create(settings);

        Data = new GameData();
        Data.Party = new Party(IndexDatabase.Instance().Parties.defaultParty);
        LoadOrCreateSystemMemory();
    }

    public void Start() {
        Global.Instance().Dispatch.RegisterListener(MapManager.EventTeleport, OnTeleport);
    }

    public void OnDestroy() {
        Global.Instance()?.Dispatch?.UnregisterListener(MapManager.EventTeleport, OnTeleport);
    }

    public void SaveToSlot(int slot) {
        Data.SaveVersion = CurrentSaveVersion;
        Data.SavedAt = UIUtils.CurrentTimestamp();
        Data.MapPath = Global.Instance().Maps.ActiveMap.InternalName;
        Data.MapLocation = Global.Instance().Maps.Avatar.GetComponent<MapEvent>().Position;
        WriteJsonToFile(Data, FilePathForSlot(slot));

        SystemData.LastSaveSlot = slot;
        SystemData.SaveInfo[slot] = new SaveInfoData(Data);
        SaveSystemMemory();
    }

    public void LoadFromLastSaveSlot() {
        SetGameData(LoadGameDataForSlot(SystemData.LastSaveSlot));
    }

    public GameData LoadGameDataForSlot(int slot) {
        var fileName = FilePathForSlot(slot);
        if (File.Exists(fileName)) {
            return ReadJsonFromFile<GameData>(fileName);
        } else {
            return null;
        }
    }

    public bool DoSavedGamesExist() {
        return SystemData.LastSaveSlot > -1;
    }

    public void SaveSystemMemory() {
        WriteJsonToFile(SystemData, GetSystemMemoryFilepath());
    }

    public IEnumerator LoadGameRoutine(int slot) {
        Data = LoadGameDataForSlot(slot);
        SceneManager.LoadScene("Map2D", LoadSceneMode.Single);
        var transition = IndexDatabase.Instance().Transitions.GetData(FadeComponent.DefaultTransitionTag);
        yield return Global.Instance().Maps.Camera.GetComponent<FadeComponent>().FadeRoutine(transition.GetFadeOut(), false, 0.0f);
        yield return Global.Instance().Maps.TeleportRoutine(Data.MapPath, Data.MapLocation, OrthoDir.South, true);
        yield return Global.Instance().Maps.Camera.GetComponent<FadeComponent>().FadeRoutine(transition.GetFadeIn(), true);
    }

    /// <remarks>
    /// will instantly change globals and avatar to match the memory, assumes the main scene is the current scene
    /// </remarks>
    private void SetGameData(GameData data) {
        Data = data;
        Global.Instance().Maps.Avatar.Chara.SetAppearanceByTag(data.Party.Leader.FieldSpriteTag);
    }

    private void WriteJsonToFile(object toSerialize, string fileName) {
        FileStream file = File.Open(fileName, FileMode.Create);
        StreamWriter writer = new StreamWriter(file);
        serializer.Serialize(writer, toSerialize);
        writer.Flush();
        writer.Close();
    }

    private T ReadJsonFromFile<T>(string fileName) {
        var json = File.ReadAllText(fileName);
        return serializer.Deserialize<T>(new JsonTextReader(new StringReader(json)));
    }

    private string GetSystemMemoryFilepath() {
        return Application.persistentDataPath + "/" + SystemMemoryName;
    }

    private string FilePathForSlot(int slot) {
        string fileName = Application.persistentDataPath + "/";
        fileName += slot.ToString();
        fileName += SaveGameSuffix;
        return fileName;
    }

    private void LoadOrCreateSystemMemory() {
        string path = GetSystemMemoryFilepath();
        if (File.Exists(path)) {
            SystemData = ReadJsonFromFile<SystemData>(path);
        } else {
            SystemData = new SystemData();
        }
    }

    private void OnTeleport(object payload) {
        Data.OnTeleportTo((Map)payload);
    }
}
