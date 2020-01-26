using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FieldSpriteImage : MonoBehaviour {

    [SerializeField] private FieldSpritesheetComponent spritesheet = null;
    [SerializeField] private OrthoDir facing = OrthoDir.South;

    private Image image;
    private Image Image {
        get {
            if (image == null) {
                image = GetComponent<Image>();
            }
            return Image;
        }
    }

    public void Populate(string tag) {
        spritesheet.SetByTag(tag);
        Image.sprite = spritesheet.FrameForDirection(facing);
    }
}
