[UnityEngine.CreateAssetMenu(fileName="EffectDefend", menuName="Data/")]
public class EffectDefendData : EffectAllyTargetData {

    [UnityEngine.Tooltip("Defend item name - as in, \"<char> blocks by <name>\", leave blank to use parent it" +
"em")]
    public string defendName;

    [UnityEngine.Tooltip("Shield value - raises evasion rate by this amount, usually 0-100")]
    public int shielding = 0;

    [UnityEngine.Tooltip("Trigger types - when any of these are taken, counter will trigger")]
    public DamageType[] triggerTypes;

    [UnityEngine.Tooltip("Counter - an offensive ability to launch when triggered")]
    public AbilEffectData warhead;

    [UnityEngine.Tooltip("Stat modifiers - applied to the defended characters for the turn")]
    public StatModData stats;

    [UnityEngine.Tooltip("Other flags")]
    public DefenseFlag[] effects;




}
