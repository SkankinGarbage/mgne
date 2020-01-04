[UnityEngine.CreateAssetMenu(fileName="SoundManager", menuName="Data/Audio/")]
public class SoundManagerData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Sound effect entries")]
    public SoundEffectEntryData soundEffectEntries;
    
    [UnityEngine.Tooltip("Emu track entries")]
    public EmuMusicEntryData emuMusicEntries;
    
    [UnityEngine.Tooltip("Loaded music entries")]
    public LoadedMusicEntryData loadedMusicEntries;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
