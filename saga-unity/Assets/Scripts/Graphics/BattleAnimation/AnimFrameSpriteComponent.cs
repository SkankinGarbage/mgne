using UnityEngine;
using UnityEngine.UI;

public class AnimFrameSpriteComponent : MonoBehaviour {

    private const string BattleAnimDirectory = "Sprites/BattleAnim/";

    [SerializeField] private new SpriteRenderer renderer = null;

    public void Populate(BattleStepData data, Vector3 origin) {
        renderer.sprite = SpriteForStep(data);

        var transform = renderer.transform;
        transform.position = new Vector3(origin.x, origin.y, transform.position.z);

        var transform2 = renderer.transform;
        if (data.rotation == RotationType.ROTATION_ENABLED) {
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 4) * 90);
        } else {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        ApplyOffset(new Vector3(data.x, data.y, 0));
    }

    public void ApplyOffset(Vector3 offset) {
        transform.localPosition = transform.localPosition + new Vector3(offset.x / 16, offset.y * -1/16, 0);
    }

    public static Sprite SpriteForStep(BattleStepData data) {
        var spriteName = data.sprite;
        if (spriteName.StartsWith("battle_anim/")) {
            spriteName = spriteName.Substring("battle_anim/".Length);
        }
        if (spriteName.EndsWith(".png")) {
            spriteName = spriteName.Substring(0, spriteName.IndexOf(".png"));
        }
        return Resources.Load<Sprite>(BattleAnimDirectory + spriteName);
    }
}
