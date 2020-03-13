using UnityEngine;
using System.Collections;

public class BattlerDoll : MonoBehaviour {

    public FieldSpriteImage image;

    public void Populate(Unit unit) {
        image.Populate(unit.FieldSpriteTag);
        image.animates = unit.IsAlive;
    }
}
