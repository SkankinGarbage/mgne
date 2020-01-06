[System.Serializable]
public class EmuMusicEntryData {

    [UnityEngine.Tooltip("Reference key - can be used to refer to this key in-game")]
    public string refKey;

    [UnityEngine.Tooltip("File - .gbs containing the relevant music")]
    public string gbsPath;

    [UnityEngine.Tooltip("Track number - index of this track in the gbs file, from 0")]
    public int track;
}
