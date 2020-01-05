using UnityEngine;
using System.Collections;
using System;

public enum DamageType {
    
    [DamageType(    new StatTag[] { },                          new StatTag[] { StatTag.RESIST_DAMAGE },                            new StatTag[] { })]                     PHYSICAL,
    [DamageType(    new StatTag[] { StatTag.IMMUNE_WEAPON },    new StatTag[] { StatTag.RESIST_DAMAGE, StatTag.RESIST_WEAPON },     new StatTag[] { })]                     WEAPON,

    [DamageType(    new StatTag[] { StatTag.RESIST_FIRE},       new StatTag[] { },                                                  new StatTag[] { StatTag.WEAK_FIRE})]    FIRE,
    [DamageType(    new StatTag[] { StatTag.RESIST_ICE},        new StatTag[] { },                                                  new StatTag[] { StatTag.WEAK_ICE})]     ICE,
    [DamageType(    new StatTag[] { StatTag.RESIST_EARTH},      new StatTag[] { },                                                  new StatTag[] { StatTag.WEAK_EARTH})]   EARTH,
    [DamageType(    new StatTag[] { StatTag.RESIST_THUNDER},    new StatTag[] { },                                                  new StatTag[] { StatTag.WEAK_THUNDER})] THUNDER,

    [DamageType(    new StatTag[] { },                          new StatTag[] { StatTag.RESIST_TYPELESS },                          new StatTag[] { })]                     TYPELESS,
}


public class DamageTypeAttribute : Attribute {

    public StatTag[] resistFlags;
    public StatTag[] weakFlags;
    public StatTag[] immuneFlags;

    internal DamageTypeAttribute(StatTag[] immuneFlags, StatTag[] resistFlags, StatTag[] weakFlags) {
        this.resistFlags = resistFlags;
        this.weakFlags = weakFlags;
        this.immuneFlags = immuneFlags;
    }
}

public static class DamageTypeExtensions {

    public static StatTag[] ResistFlags(this DamageType damageType) { return damageType.GetAttribute<DamageTypeAttribute>().resistFlags; }
    public static StatTag[] WeakFlags(this DamageType damageType) { return damageType.GetAttribute<DamageTypeAttribute>().weakFlags; }
    public static StatTag[] ImmuneFlags(this DamageType damageType) { return damageType.GetAttribute<DamageTypeAttribute>().immuneFlags; }
}
