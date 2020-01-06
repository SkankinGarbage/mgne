[System.Serializable]
public class StatModData : MainSchema {

    [UnityEngine.Tooltip("Numeric stat modifiers")]
    public StatEntryData[] stats;

    public StatTag[] flags;
}
