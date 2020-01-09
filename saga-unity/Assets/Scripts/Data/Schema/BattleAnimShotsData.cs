[UnityEngine.CreateAssetMenu(fileName="BattleAnimShots", menuName="Data/Graphics/")]
public class BattleAnimShotsData : BattleAnimData {

    [Newtonsoft.Json.JsonConverter(typeof(LinkerDeserializer))]
    [UnityEngine.Tooltip("Animation - the individual shot animation")]
    public BattleAnimStripData anim;

    [UnityEngine.Tooltip("Count - shots to fire in total")]
    public int count;

    [UnityEngine.Tooltip("Delay - interval between each shot, in seconds")]
    public float delay;

    [UnityEngine.Tooltip("Columns - this many shots appear in a row")]
    public int cols;

    [UnityEngine.Tooltip("Horizontal padding - two shots have this many pixels between them")]
    public float padX;

    [UnityEngine.Tooltip("Vertical padding - two rows of shots have this many pixels between them")]
    public float padY;

    [UnityEngine.Tooltip("Max vertical gain - each shot appears +/- by this many pixels")]
    public float gainX;

    [UnityEngine.Tooltip("Max horizontal gain - each shot appears +/- by this many pixels")]
    public float gainY;

    [UnityEngine.Tooltip("Horizontal jitter - each shot height is offset by this many pixels")]
    public float jitterX;

    [UnityEngine.Tooltip("Vertical jitter - each shot height is offset by this many pixels")]
    public float jitterY;
}
