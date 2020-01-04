[UnityEngine.CreateAssetMenu(fileName="BattleAnimShader", menuName="Data/Graphics/")]
public class BattleAnimShaderData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Shader - will apply to all enemies")]
    public ShaderData shader;
    
    [UnityEngine.Tooltip("Duration - in seconds")]
    public float duration;
    
    [UnityEngine.Tooltip("Scope - will still apply to all enemies")]
    public @object scope = "ENEMY_AREA";
    
    [UnityEngine.Tooltip("Sound effect - plays with the animation in battle only")]
    public string sound;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
