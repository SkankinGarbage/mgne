using UnityEngine;

[CreateAssetMenu(fileName = "IndexDatabase", menuName = "Data/IndexDatabase")]
public class IndexDatabase : ScriptableObject {

    public TransitionIndexData Transitions;
    public FadeIndexData Fades;
    public SoundEffectIndexData SFX;
    public BGMIndexData BGM;
    public FieldSpriteIndexData FieldSprites;
    public CombatItemIndex CombatItems;
    public PartyIndex Parties;
    public UnitIndex Units;
    public StatusIndex Statuses;
    public RecruitIndex Recruits;
    public MeatGroupIndex MeatGroups;
    public ShopIndex Shops;
    public CollectableIndex Collectables;
    public EncounterSetIndex EncounterSets;
    public TerrainEncounterSetIndex TerrainEncounterSets;

    public static IndexDatabase Instance() {
        return Resources.Load<IndexDatabase>("Database/Database");
    }
}
