using System.Collections.Generic;

[System.Serializable]
public class SystemData {

    public int LastSaveSlot { get; set; } = -1;

    public SaveInfoData[] SaveInfo { get; private set; } = new SaveInfoData[SerializationManager.SaveSlotCount];

    public Setting<bool> SettingFullScreen { get; private set; } =               new Setting<bool>(false);
    public Setting<float> SettingMusicVolume { get; private set; } =             new Setting<float>(0.9f);
    public Setting<float> SettingSoundEffectVolume { get; private set; } =       new Setting<float>(0.9f);
}
