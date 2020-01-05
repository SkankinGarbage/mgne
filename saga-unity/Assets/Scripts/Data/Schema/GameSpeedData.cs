[UnityEngine.CreateAssetMenu(fileName="GameSpeed", menuName="Data/Settings/")]
public class GameSpeedData : UnityEngine.ScriptableObject {

    [UnityEngine.Tooltip("Target render rate, in frames per second. At rates lower than this, the game will" +
" slow down.")]
    public int framerate;

    [UnityEngine.Tooltip("Base delay, in seconds. Every time the player makes a move, this much time elapse" +
"s before they show up at their next square.")]
    public float delay;

    
    
}
