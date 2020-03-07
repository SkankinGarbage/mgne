using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnimFrameSpriteComponent : MonoBehaviour {

    private const string BattleAnimDirectory = "Sprites/BattleAnim/";

    private Image image;
    private Image Image {
        get {
            if (image == null) {
                image = GetComponent<Image>();
            }
            return image;
        }
    }

    public void Populate(BattleStepData data, Vector2 origin) {
        var spriteName = data.sprite;
        if (spriteName.StartsWith("battle_anim/")) {
            spriteName = spriteName.Substring("battle_anim/".Length);
        }
        if (spriteName.EndsWith(".png")) {
            spriteName = spriteName.Substring(0, spriteName.IndexOf(".png"));
        }
        var sprite = Resources.Load<Sprite>(BattleAnimDirectory + spriteName);
        Image.sprite = sprite;
        Image.SetNativeSize();

        var transform = GetComponent<RectTransform>();
        transform.anchoredPosition = transform.anchoredPosition + new Vector2(data.x, data.y);

        if (data.rotation == RotationType.ROTATION_ENABLED) {
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 4) * 90);
        }
    }
}
