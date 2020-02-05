using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MiniUnitCellView : MonoBehaviour {

    [SerializeField] private Text statusText = null;
    [SerializeField] private FieldSpriteImage sprite = null;
    [SerializeField] private new Text name = null;

    public void Populate(Unit unit, PointerLayer pointers) {
        if (unit.Status == null || unit.IsDead) {
            statusText.text = unit[StatTag.HP] + "/" + unit[StatTag.MHP];
        } else {
            statusText.text = unit.Status.ToString();
        }
        if (name != null) {
            name.text = unit.Name;
        }
        sprite.Populate(unit);

        if (pointers != null && GetComponent<SelectableCell>()?.selectedState.GetComponent<PointerComponent>() != null) {
            GetComponent<SelectableCell>()?.selectedState.GetComponent<PointerComponent>().Populate(pointers);
        }
    }
}
