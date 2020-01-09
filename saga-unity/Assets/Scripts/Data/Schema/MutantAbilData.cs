[System.Serializable]
public class MutantAbilData {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Ability")]
    public CombatItemData abil;

    [UnityEngine.Tooltip("Chance - 0-100, 50 indicates this abil is learned 50% of the time it\'s selected, " +
"100 is always, 2 or 3 is really rare")]
    public int chance;

    [UnityEngine.Tooltip("Level - probability of learning this abil is halved if fighting monsters below th" +
"is level")]
    public int level;
}
