[UnityEngine.CreateAssetMenu(fileName="BattleAnimSound", menuName="Data/Graphics/")]
public class BattleAnimSoundData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Duration - in seconds, duration of the sfx, sorry")]
    public float duration;
    
    [UnityEngine.Tooltip("Sound effect - plays with the animation in battle only")]
    public string sound;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
