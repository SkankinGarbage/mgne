using Mgne1;

[UnityEngine.CreateAssetMenu(fileName="MeatGroup", menuName="Data/Rpg")]
public class MeatGroupData : MainSchema {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    public MonsterFamilyData[] families;
}
