﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(FieldSpritesheetComponent))]
public class FieldSpriteImage : MonoBehaviour {

    [SerializeField] private OrthoDir facing = OrthoDir.South;

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

    public void Populate(string tag) {
        Spritesheet.SetByTag(tag);
        Image.sprite = Spritesheet.FrameForDirection(facing);
    }
}