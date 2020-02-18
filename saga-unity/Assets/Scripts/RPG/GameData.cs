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

    public void AddGP(int gp) { GP += gp; }
    public void DeductGP(int gp) { GP -= gp; }

    public GameData() {
        GP = 999;
        LocationName = "Debug";
        Party = new Party(IndexDatabase.Instance().Parties.defaultParty);
        Inventory = new Inventory(InventoryCapacity);
    }

    public void OnTeleportTo(Map map) {
        LocationName = map.MapName;
        CurrentBGMKey = map.BgmKey;
    }
}
