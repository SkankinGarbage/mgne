using System.Collections.Generic;
/// <summary>
/// The generic stuff that used to be attached to the player party, such as location, gp, etc.
/// </summary>
[System.Serializable]
public class GameData {

    private const int InventoryCapacity = 10;

    public Party Party { get; private set; }
    public Inventory Inventory { get; private set; }
    public int GP { get; private set; }
    public string LocationName { get; private set; }
    public string CurrentBGMKey { get; private set; }

    public Dictionary<string, int> Variables { get; private set; }
    public Dictionary<string, bool> Switches { get; private set; }

    // meta info
    public int SaveVersion { get; set; }
    public double SavedAt { get; set; }

    public void AddGP(int gp) { GP += gp; }
    public void DeductGP(int gp) { GP -= gp; }

    public GameData() {
        GP = 999;
        LocationName = "Debug";
        Party = new Party(IndexDatabase.Instance().Parties.defaultParty);
        Inventory = new Inventory(InventoryCapacity);
        Variables = new Dictionary<string, int>();
        Switches = new Dictionary<string, bool>();
    }

    public void OnTeleportTo(Map map) {
        LocationName = map.MapName;
        CurrentBGMKey = map.BgmKey;
    }

    public bool GetSwitch(string switchName) {
        if (!Switches.ContainsKey(switchName)) {
            return false;
        }
        return Switches[switchName];
    }

    public void SetSwitch(string switchName, bool value) {
        Switches[switchName] = value;
    }

    public int GetVariable(string variableName) {
        if (!Variables.ContainsKey(variableName)) {
            Variables[variableName] = 0;
        }
        return Variables[variableName];
    }

    public void IncrementVariable(string variableName) {
        Variables[variableName] = GetVariable(variableName) + 1;
    }

    public void DecrementVariable(string variableName) {
        Variables[variableName] = GetVariable(variableName) - 1;
    }
}
