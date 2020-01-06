[System.Serializable]
public class StatSetData {

    [UnityEngine.Tooltip("HP")]
    public int hp;

    [UnityEngine.Tooltip("MHP")]
    public int mhp;

    [UnityEngine.Tooltip("STR")]
    public int str;

    [UnityEngine.Tooltip("DEF")]
    public int def;

    [UnityEngine.Tooltip("AGI")]
    public int agi;

    [UnityEngine.Tooltip("MANA")]
    public int mana;

    [UnityEngine.Tooltip("Flags")]
    public StatTag[] flags;
}
