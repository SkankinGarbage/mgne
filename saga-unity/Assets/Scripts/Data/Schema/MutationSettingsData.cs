[UnityEngine.CreateAssetMenu(fileName="MutationSettings", menuName="Data/Rpg/")]
public class MutationSettingsData : MainSchema {

    [UnityEngine.Tooltip("Available mutant abilities")]
    public MutantAbilData abils;

    [UnityEngine.Tooltip("Mutation chance - probability 0-100 that mutants mutate after a battle")]
    public int mutationChance;

    [UnityEngine.Tooltip("Min HP - minimum health gained in an MHP level up")]
    public int minHealth;

    [UnityEngine.Tooltip("Max HP - maximum health gained in an MHP level up")]
    public int maxHealth;

    [UnityEngine.Tooltip("HP weight - weight of an MHP gain relative to other gains")]
    public int weightHp;

    [UnityEngine.Tooltip("STR weight - weight of a STR gain relative to other gains")]
    public int weightStr;

    [UnityEngine.Tooltip("DEF weight - weight of a DEF gain relative to other gains")]
    public int weightDef;

    [UnityEngine.Tooltip("AGI weight - weight of an AGI gain relative to other gains")]
    public int weightAgi;

    [UnityEngine.Tooltip("MAN weight - weight of a MAN gain relative to other gains")]
    public int weightMana;

    [UnityEngine.Tooltip("Ability weight - weight of learning an ability relative to other gains")]
    public int weightAbil;
}
