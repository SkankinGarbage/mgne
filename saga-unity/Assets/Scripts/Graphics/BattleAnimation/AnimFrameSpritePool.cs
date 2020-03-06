using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public class AnimFrameSpritePool : MonoBehaviour {

    [SerializeField] private AnimFrameSpriteComponent framePrefab = null;

    private ObjectPool pool;
    private ObjectPool Pool {
        get {
            if (pool == null) {
                pool = GetComponent<ObjectPool>();
            }
            return pool;
        }
    }

    public AnimFrameSpriteComponent GetFrame(BattleStepData data, Vector2 origin) {
        var frame = Pool.GetInstance().GetComponent<AnimFrameSpriteComponent>();
        frame.Populate(data, origin);
        return frame;
    }

    public void DisposeFrame(AnimFrameSpriteComponent frame) {
        Pool.FreeInstance(frame.gameObject);
    }
}
