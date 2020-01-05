using UnityEngine;

public enum LethalityType {

    DEATH,
    DEADLY,
    NON_DEADLY,

}

public enum RecoverType {

    RECOVER_AFTER_BATTLE,
    PERSIST_UNTIL_HEAL,

}

public enum AbilityType {

    ITEM,
    ABILITY,

}

public enum EquipmentFlag {

    ARMOR,
    GAUNTLET,
    HELMET,
    SHOE,

}

public enum AllyProjectorType {

    USER,
    SINGLE_ALLY,
    PLAYER_PARTY_ENEMY_GROUP,
    ALLY_PARTY,

}

public enum OffenseProjectorType {

    SINGLE_ENEMY,
    GROUP_ENEMY,
    ALL_ENEMY,

}

public enum OffenseFlag {

    KILLS_USER,
    ONLY_AFFECT_UNDEAD,
    ONLY_AFFECT_HUMANS,
    DRAIN_LIFE,
    CRITICAL_ON_WEAKNESS,
    IGNORE_RESISTANCES,
    STUNS_ON_HIT,

}

public enum DefenseFlag {

    BLOCKS_TRIGGERING_DAMAGE,
    REFLECTS_TRIGGERING_ATTACK,

}

public enum UseRestoreType {

    NO_USE_RESTORE,
    RESTORES_USES,

}

public enum ShaderScopeType {

    PORTRAIT,
    ENEMY_AREA,

}

public enum RotationType {

    ROTATION_ENABLED,
    ROTATION_DISABLED,

}

public enum MissType {

    CAN_MISS,
    ALWAYS_HITS,

}
