using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(FieldSpritesheetComponent))]
public class FieldSpriteImage : MonoBehaviour {

    public OrthoDir facing = OrthoDir.South;

    public bool animates = false;
    public int stepsPerSecond = 2;

    private FieldSpritesheetComponent spritesheet;
    public FieldSpritesheetComponent Spritesheet {
        get {
            if (spritesheet == null) {
                spritesheet = GetComponent<FieldSpritesheetComponent>();
            }
            return spritesheet;
        }
    }
    
    private Image image;
    private Image Image {
        get {
            if (image == null) {
                image = GetComponent<Image>();
            }
            return image;
        }
    }

    private new string tag;
    private float elapsed;

    public void Populate(string tag) {
        this.tag = tag;
        Spritesheet.SetByTag(tag);
        if (tag != null) {
            Image.sprite = Spritesheet.FrameForDirection(facing);
        }
    }

    public void Populate(Unit unit) {
        Populate(unit.FieldSpriteTag);
        facing = unit.IsDead ? OrthoDir.North : OrthoDir.South;
    }

    public void Update() {
        if (animates) {
            elapsed += Time.deltaTime;
        } else {
            elapsed = 0;
        }
        Image.sprite = Spritesheet.FrameBySlot((int)(elapsed * stepsPerSecond) % Spritesheet.StepCount, facing);
    }
}
