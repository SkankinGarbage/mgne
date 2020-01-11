[UnityEngine.CreateAssetMenu(fileName="BattleAnimShader", menuName="Data/Graphics")]
public class BattleAnimShaderData : BattleAnimData {

    [UnityEngine.Tooltip("Shader - will apply to all enemies")]
    // TODO: mgn
    public string shader;

    [UnityEngine.Tooltip("Duration - in seconds")]
    public float duration;

    [UnityEngine.Tooltip("Scope - will still apply to all enemies")]
    public ShaderScopeType scope = ShaderScopeType.ENEMY_AREA;
}
