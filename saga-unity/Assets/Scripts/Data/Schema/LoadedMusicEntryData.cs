[UnityEngine.CreateAssetMenu(fileName="LoadedMusicEntry", menuName="Data/")]
public class LoadedMusicEntryData : UnityEngine.ScriptableObject {

    [UnityEngine.Tooltip("Reference key - can be used to refer to this key in-game")]
    public string refKey;

    [UnityEngine.Tooltip("File - .mp3, .ogg etc containing the relevant music")]
    public string path;
}
