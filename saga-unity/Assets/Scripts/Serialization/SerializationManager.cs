using System.IO;
using UnityEngine;

public class SerializationManager : MonoBehaviour {

    public const int SaveSlotCount = 5;

    private const int CurrentSaveVersion = 1;
    private const string SystemMemoryName = "mgne2.sav";
    private const string SaveGameSuffix = ".sav";

    public GameData Data { get; private set; }
    public SystemData System { get; private set; }

    public void Start() {
        Global.Instance().Dispatch.RegisterListener(MapManager.EventTeleport, OnTeleport);
        LoadOrCreateSystemMemory();
    }

    public void InitializeData() {
        Data = new GameData();
    }

    public void OnDestroy() {
        Global.Instance()?.Dispatch?.UnregisterListener(MapManager.EventTeleport, OnTeleport);
    }

    public void SaveToSlot(int slot) {
        Data.SaveVersion = CurrentSaveVersion;
        WriteJsonToFile(Data, FilePathForSlot(slot));

        System.LastSaveSlot = slot;
        System.SaveInfo[slot] = new SaveInfoData(Data);
        SaveSystemMemory();
    }

    public void LoadFromLastSaveSlot() {
        SetGameData(LoadGameDataForSlot(System.LastSaveSlot));
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
        return System.LastSaveSlot > -1;
    }

    public void SaveSystemMemory() {
        WriteJsonToFile(System, GetSystemMemoryFilepath());
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
        writer.Write(JsonUtility.ToJson(toSerialize));
        writer.Flush();
        writer.Close();
        file.Close();
    }


    private T ReadJsonFromFile<T>(string fileName) {
        string json = File.ReadAllText(fileName);
        T result = JsonUtility.FromJson<T>(json);
        return result;
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
            System = ReadJsonFromFile<SystemData>(path);
        } else {
            System = new SystemData();
        }
    }

    private void OnTeleport(object payload) {
        Data.OnTeleportTo((Map)payload);
    }
}
