[UnityEngine.CreateAssetMenu(fileName="Emitter", menuName="Data/Graphics/")]
public class EmitterData : UnityEngine.ScriptableObject {
    
    [UnityEngine.Tooltip("Mode - will this emitter fire once or keep firing?")]
    public @object mode;
    
    [UnityEngine.Tooltip("Reflection mode - how do particles interact with walls?")]
    public @object reflect;
    
    [UnityEngine.Tooltip("Bounce mode - particles can bounce on the floor")]
    public @object bounce;
    
    [UnityEngine.Tooltip("Rate of fire- how many particles to fire (possibly per second)")]
    public int count;
    
    [UnityEngine.Tooltip("Velocity - maximum velocity of a particle (in px/s)")]
    public int velocity;
    
    [UnityEngine.Tooltip("Rotation velocity - maximum rotational velocity of a particle (in rotations/s)")]
    public int rotationalVelocity;
    
    [UnityEngine.Tooltip("Min lifetime - minimum time a particle persists, in seconds")]
    public float minLife;
    
    [UnityEngine.Tooltip("Max lifetime - maximum time a particle persists, in seconds")]
    public float maxLife;
    
    [UnityEngine.Tooltip("Notes and whatnot on this object")]
    public string description;
}
