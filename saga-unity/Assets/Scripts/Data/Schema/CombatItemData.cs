[UnityEngine.CreateAssetMenu(fileName="CombatItem", menuName="Data/Rpg/")]
public class CombatItemData : MainSchema {

    [UnityEngine.Tooltip("Ability name - displayed in-game, potentially with $A special char codes")]
    public string abilityName;

    [UnityEngine.Tooltip("Description - used for noob mode")]
    public string itemDescription;

    [UnityEngine.Tooltip("Type")]
    public AbilityType type;

    [UnityEngine.Tooltip("Uses - or zero for infinite uses")]
    public int uses = 0;

    [UnityEngine.Tooltip("Cost - or zero for unsellable, halved for resale rate")]
    public int cost = 0;

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Animation - graphical effect that plays when this item is used in battle")]
    public BattleAnimData anim;

    [UnityEngine.Tooltip("Equipment types - characters can\'t equip two items that share a flag")]
    public EquipmentFlag[] equip;

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Effect - what happens when this applies")]
    public AbilEffectData warhead;

    [UnityEngine.Tooltip("Tier - mostly cosmetic, but can be used to autogenerate robostats")]
    public int tier;

    [Newtonsoft.Json.JsonConverter(typeof(StatModDeserializer))]
    [UnityEngine.Tooltip("Robo stats - these boosts are granted to robots that equip this")]
    public StatSet robostats;
}
