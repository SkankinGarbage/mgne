using Newtonsoft.Json;
using System.IO;
using UnityEngine;

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

    /// <remarks>
    /// will instantly change globals and avatar to match the memory, assumes the main scene is the current scene
    /// </remarks>
    private void SetGameData(GameData data) {
        Data = data;
        Global.Instance().Maps.avatar.Chara.SetAppearanceByTag(data.Party.Leader.FieldSpriteTag);
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
