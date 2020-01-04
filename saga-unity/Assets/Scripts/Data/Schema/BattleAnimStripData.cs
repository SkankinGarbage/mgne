[UnityEngine.CreateAssetMenu(fileName="BattleAnimStrip", menuName="Data/Graphics/")]
public class BattleAnimStripData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Steps")]
    public BattleStepData steps;
    
    [UnityEngine.Tooltip("Sound effect - plays with the animation in battle only")]
    public string sound;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
