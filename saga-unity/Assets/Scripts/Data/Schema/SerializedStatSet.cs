using System;
using System.Collections.Generic;

[Serializable]
public class SerializedStatSet {

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

    public List<StatTag> flagKeys;
    public List<float> flagValues;

    public SerializedStatSet(StatSet stats) {
        hp = (int) stats[StatTag.HP];
        mhp = (int)stats[StatTag.MHP];
        str = (int)stats[StatTag.STR];
        def = (int)stats[StatTag.DEF];
        agi = (int)stats[StatTag.AGI];
        mana = (int)stats[StatTag.MANA];

        flagKeys = new List<StatTag>();
        flagValues = new List<float>();

        foreach (StatTag tag in Enum.GetValues(typeof(StatTag))) {
            Stat stat = Stat.Get(tag);
            if (stat.UseBinaryEditor) {
                flagKeys.Add(tag);
                flagValues.Add(stats[tag]);
            }
        }
    }
}
