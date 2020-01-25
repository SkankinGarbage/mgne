using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "FieldSpriteDataIndexData", menuName = "Data/Index/FieldSpriteData")]
public class FieldSpriteIndexData : GenericIndex<FieldSpriteData> {

}

[Serializable]
public class FieldSpriteData : GenericDataObject {

    public Texture2D spriteSheet;
    public bool autoAnimate = true;
}
