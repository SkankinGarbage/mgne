[UnityEngine.CreateAssetMenu(fileName="BattleStep", menuName="Data/")]
public class BattleStepData : UnityEngine.ScriptableObject {

    [UnityEngine.Tooltip("Component sprite file")]
    public string sprite;

    [UnityEngine.Tooltip("Start - when this component appears, in s relative to beginning")]
    public float start;

    [UnityEngine.Tooltip("Duration - how long this sprite appears, in s")]
    public float duration;

    [UnityEngine.Tooltip("Appear x - x-coord the sprite\'s center appears at relative to the center of the e" +
"nemy battle portrait")]
    public float x;

    [UnityEngine.Tooltip("Appear y - y-coord the sprite\'s center appears at relative to the center of the e" +
"nemy battle portrait")]
    public float y;

    [UnityEngine.Tooltip("Rotation type - enable if the result of rotation is good-looking and distinct")]
    public RotationType rotation = RotationType.ROTATION_DISABLED;
}
