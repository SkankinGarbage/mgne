using UnityEngine;
using System.Collections;

/// <summary>
/// The generic stuff that used to be attached to the player party, such as location, gp, etc.
/// </summary>
public class GameData {

    public Party Party { get; private set; }
    public int GP { get; private set; }
    public string LocationName { get; private set; }
    public string CurrentBGMKey { get; private set; }

    public void AddGP(int gp) { GP += gp; }
    public void DeductGP(int gp) { GP -= gp; }

    public GameData() {
        GP = 0;
        LocationName = "";
    }

    public void OnTeleportTo(Map map) {
        LocationName = map.MapName;
        CurrentBGMKey = map.BgmKey;
    }
}
