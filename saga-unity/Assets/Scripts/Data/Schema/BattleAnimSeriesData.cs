[UnityEngine.CreateAssetMenu(fileName="BattleAnimSeries", menuName="Data/Graphics/")]
public class BattleAnimSeriesData : UnityEngine.ScriptableObject {

    [UnityEngine.Tooltip("Animation - this is the strip that plays a bunch of times")]
    public BattleAnimStripData anim;

    [UnityEngine.Tooltip("Count - how many to play total")]
    public int count = "6";

    [UnityEngine.Tooltip("Concurrent count - how many play at once")]
    public int concurrent = "1";

    [UnityEngine.Tooltip("Span - each anim will appear in an n x n area around target")]
    public int span;

    [UnityEngine.Tooltip("Granularity - each anim will appear on an n by n grid of this size")]
    public int granularity = "16";

    [UnityEngine.Tooltip("Sound effect - plays with the animation in battle only")]
    public string sound;

    
    
}
