[UnityEngine.CreateAssetMenu(fileName="MonsterFamily", menuName="Data/Rpg/")]
public class MonsterFamilyData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Transformations to other monster families")]
    public TransformationData transformations;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
