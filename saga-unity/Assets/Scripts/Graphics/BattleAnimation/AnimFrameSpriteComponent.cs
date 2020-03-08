using UnityEngine;
using UnityEngine.UI;

public class AnimFrameSpriteComponent : MonoBehaviour {

    private const string BattleAnimDirectory = "Sprites/BattleAnim/";

    [SerializeField] private Image image = null;

    public void Populate(BattleStepData data, Vector2 origin) {
        image.sprite = SpriteForStep(data);

        var transform = image.GetComponent<RectTransform>();
        transform.anchoredPosition = transform.anchoredPosition + new Vector2(data.x, -1 * data.y);
        transform.sizeDelta = new Vector2(image.sprite.bounds.size.x, image.sprite.bounds.size.y);

        var transform2 = image.GetComponent<RectTransform>();
        if (data.rotation == RotationType.ROTATION_ENABLED) {
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 4) * 90);
        } else {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    public void ApplyOffset(Vector2 offset) {
        var transform = GetComponent<RectTransform>();
        transform.anchoredPosition = transform.anchoredPosition + offset * new Vector2(1, -1);
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
