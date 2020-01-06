[System.Serializable]
public class StatModData : UnityEngine.ScriptableObject {

    [UnityEngine.Tooltip("Numeric stat modifiers")]
    public StatEntryData[] stats;

    public StatTag[] flags;
}
