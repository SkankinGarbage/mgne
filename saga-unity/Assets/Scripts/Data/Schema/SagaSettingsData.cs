[UnityEngine.CreateAssetMenu(fileName="SagaSettings", menuName="Data/Settings/")]
public class SagaSettingsData : UnityEngine.ScriptableObject {

    [UnityEngine.Tooltip("Hero party - how the hero party starts, usually empty")]
    public PartyData heroParty;

    [UnityEngine.Tooltip("Monster settings")]
    public MonsterSettingsData monsterSettings;

    [UnityEngine.Tooltip("Mutant ability list")]
    public MutationSettingsData mutantAbils;

    
    
}
