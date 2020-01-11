[UnityEngine.CreateAssetMenu(fileName="SoundManager", menuName="Data/Audio")]
public class SoundManagerData : MainSchema {

    [UnityEngine.Tooltip("Sound effect entries")]
    public SoundEffectEntryData[] soundEffectEntries;

    [UnityEngine.Tooltip("Emu track entries")]
    public EmuMusicEntryData[] emuMusicEntries;

    [UnityEngine.Tooltip("Loaded music entries")]
    public LoadedMusicEntryData[] loadedMusicEntries;
}
