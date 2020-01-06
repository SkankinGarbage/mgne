using System;
using System.Collections;
using System.Collections.Generic;

/**
 * Stats are representing as instances of this class, eg STR is an instance of Stat that has an
 * additive mixin, int display, etc. Enums aren't powerful enough to do what we want in C#. Instead
 * there's a StatTag that hooks into this class.
 */
public class Stat {

    public CombinationStrategy combinator { get; private set; }
    public StatTag tag { get; private set; }
    public string nameShort { get; private set; }
    public bool useBinaryEditor { get; private set; }

    private static Dictionary<StatTag, Stat> stats;

    private Stat(StatTag tag, CombinationStrategy combinator, string nameShort, bool useBinaryEditor) {
        this.combinator = combinator;
        this.tag = tag;
        this.nameShort = nameShort;
        this.useBinaryEditor = useBinaryEditor;
    }

    public static Stat Get(StatTag tag) {
        if (stats == null) {
            InitializeStats();
        }
        if (!stats.ContainsKey(tag)) {
            return null;
        }
        return stats[tag];
    }

    public static Stat Get(int enumIndex) {
        return Get((StatTag)enumIndex);
    }

    private static void InitializeStats() {
        stats = new Dictionary<StatTag, Stat>();
        AddStat(StatTag.MHP);
        AddStat(StatTag.HP);
        AddStat(StatTag.STR);
        AddStat(StatTag.AGI);
        AddStat(StatTag.DEF);
        AddStat(StatTag.MANA);

        foreach (StatTag tag in new StatTag[] {     StatTag.RESIST_DAMAGE,
                StatTag.RESIST_WEAPON,
                StatTag.IMMUNE_WEAPON,

                StatTag.RESIST_BLIND,
                StatTag.RESIST_CURSE,
                StatTag.RESIST_CONFUSE,
                StatTag.RESIST_SLEEP,
                StatTag.RESIST_PARALYZE,
                StatTag.RESIST_STONE,
                StatTag.RESIST_DEATH,
                StatTag.RESIST_POISON,

                StatTag.RESIST_FIRE,
                StatTag.RESIST_ICE,
                StatTag.RESIST_THUNDER,
                StatTag.RESIST_EARTH,
                StatTag.RESIST_TYPELESS,

                StatTag.WEAK_FIRE,
                StatTag.WEAK_ICE,
                StatTag.WEAK_THUNDER,
                StatTag.WEAK_EARTH,
                StatTag.UNDEAD,

                StatTag.AMBUSHER,
                StatTag.NO_AMBUSH,

                StatTag.REGENERATING,

                StatTag.EQUIPMENT_FIX }) {
            AddFlag(tag);
        }
    }

    private static void AddStat(StatTag tag) {
        stats[tag] = new Stat(tag, CombinationAdditive.Instance(), tag.ToString(), false);
    }

    private static void AddFlag(StatTag tag) {
        stats[tag] = new Stat(tag, CombinationAdditive.Instance(), tag.ToString(), true);
    }
}
