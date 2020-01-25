using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class MemoryManager : MonoBehaviour {

    public const int CurrentSaveVersion = 3;
    public const int LowestSupportedSaveVersion = 3;

    private const string SystemMemoryName = "erebus.sav";
    private const string SaveGameSuffix = ".sav";

    private Dictionary<string, bool> switches;
    private Dictionary<string, int> variables;
    private float lastSystemSavedTimestamp;

    // this thing will be read by the dialog scene when spawning
    // if non-null, it'll be loaded automatically
    public Memory ActiveMemory { get; set; }

    // global state, unrelated to playthroughs and things like that
    // things like total play time
    public SystemMemory SystemMemory { get; set; }

    public void Awake() {
        switches = new Dictionary<string, bool>();
        variables = new Dictionary<string, int>();
        lastSystemSavedTimestamp = Time.realtimeSinceStartup;
        LoadOrCreateSystemMemory();
    }

    public bool GetSwitch(string switchName) {
        if (!switches.ContainsKey(switchName)) {
            return false;
        }
        return switches[switchName];
    }

    public void SetSwitch(string switchName, bool value) {
        switches[switchName] = value;
    }

    public int GetVariable(string variableName) {
        if (!variables.ContainsKey(variableName)) {
            variables[variableName] = 0;
        }
        return variables[variableName];
    }

    public void IncrementVariable(string variableName) {
        variables[variableName] = GetVariable(variableName) + 1;
    }

    public void DecrementVariable(string variableName) {
        variables[variableName] = GetVariable(variableName) - 1;
    }

    public void SaveToSlot(int slot) {
        Memory memory = new Memory();

        // TODO:

        // we are included in listener list, heavy lifting is in PopulateMemory
        WriteJsonToFile(memory, FilePathForSlot(slot));
        SystemMemory.lastSlotSaved = slot;
        SaveSystemMemory();
    }

    // will instantly change globals and avatar to match the memory
    // assumes the main scene is the current scene
    public void LoadMemory(Memory memory) {
        // TODO:
    }

    public void LoadFromLastSaveSlot() {
        LoadMemory(GetMemoryForSlot(SystemMemory.lastSlotSaved));
    }

    public Memory GetMemoryForSlot(int slot) {
        string fileName = FilePathForSlot(slot);
        if (File.Exists(fileName)) {
            return ReadJsonFromFile<Memory>(fileName);
        } else {
            return null;
        }
    }

    public bool AnyMemoriesExist() {
        // sort of a todo
        return GetMemoryForSlot(0) != null;
    }

    public void SaveSystemMemory() {

        // constants we keep track of
        float currentTimestamp = Time.realtimeSinceStartup;
        float deltaSeconds = currentTimestamp - lastSystemSavedTimestamp;
        lastSystemSavedTimestamp = currentTimestamp;
        SystemMemory.totalPlaySeconds += (int)Math.Round(deltaSeconds);

        // other garbage in other managers
        SystemMemory.settings = Global.Instance().Settings.ToMemory();

        WriteJsonToFile(SystemMemory, GetSystemMemoryFilepath());
    }

    public void PopulateMemory(Memory memory) {
        memory.variables = new SerialDictionary<string, int>(variables);
        memory.switches = new SerialDictionary<string, bool>(switches);
        memory.savedAt = CurrentTimestamp();
        memory.saveVersion = CurrentSaveVersion;
    }

    public void PopulateFromMemory(Memory memory) {
        // just need to handle the stuff actually stored in this manager
        switches = memory.switches.ToDictionary();
        variables = memory.variables.ToDictionary();
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

    private double CurrentTimestamp() {
        return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }

    private void LoadOrCreateSystemMemory() {
        string path = GetSystemMemoryFilepath();
        if (File.Exists(path)) {
            SystemMemory = ReadJsonFromFile<SystemMemory>(path);
            Global.Instance().Settings.PopulateFromMemory(SystemMemory.settings);
        } else {
            SystemMemory = new SystemMemory();
            Global.Instance().Settings.LoadDefaults();
        }

        // time to populate from system memory
        for (int i = 0; i < SystemMemory.maxSeenCommandsKeys.Count; i += 1) {
            variables[SystemMemory.maxSeenCommandsKeys[i]] = SystemMemory.maxSeenCommandsValues[i];
        }
    }

    private string GetSystemMemoryFilepath() {
        return Application.persistentDataPath + "/" + SystemMemoryName;
    }

    private string FilePathForSlot(int slot) {
        string fileName = Application.persistentDataPath + "/";
        fileName += Convert.ToString(slot);
        fileName += SaveGameSuffix;
        return fileName;
    }

    private DateTime TimestampToDateTime(double timestamp) {
        System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();
        return dtDateTime;
    }
}
