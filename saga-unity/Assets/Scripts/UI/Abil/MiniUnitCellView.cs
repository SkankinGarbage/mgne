using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MiniUnitCellView : MonoBehaviour {

    [SerializeField] private Text statusText = null;
    [SerializeField] private FieldSpriteImage sprite = null;

    public void Populate(Unit unit) {
        if (unit.Status == null || unit.IsDead) {
            statusText.text = unit[StatTag.HP] + "/" + unit[StatTag.MHP];
        } else {
            statusText.text = unit.Status.ToString();
        }
        sprite.Populate(unit);
    }
}
