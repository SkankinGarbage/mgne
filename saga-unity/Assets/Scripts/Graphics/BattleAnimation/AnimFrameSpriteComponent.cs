using UnityEngine;
using UnityEngine.UI;

public class AnimFrameSpriteComponent : MonoBehaviour {

    private const string BattleAnimDirectory = "Sprites/BattleAnim/";

    [SerializeField] private Image image = null;

    public void Populate(BattleStepData data, Vector2 origin) {
        var spriteName = data.sprite;
        if (spriteName.StartsWith("battle_anim/")) {
            spriteName = spriteName.Substring("battle_anim/".Length);
        }
        if (spriteName.EndsWith(".png")) {
            spriteName = spriteName.Substring(0, spriteName.IndexOf(".png"));
        }
        var sprite = Resources.Load<Sprite>(BattleAnimDirectory + spriteName);
        image.sprite = sprite;
        image.SetNativeSize();

        var transform = image.GetComponent<RectTransform>();
        transform.anchoredPosition = transform.anchoredPosition + new Vector2(data.x, data.y);

        if (data.rotation == RotationType.ROTATION_ENABLED) {
            var transform2 = image.GetComponent<RectTransform>();
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 4) * 90);
        }
    }

    public void ApplyOffset(Vector2 offset) {
        var transform = GetComponent<RectTransform>();
        transform.anchoredPosition = transform.anchoredPosition + offset;
    }
}
