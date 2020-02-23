using UnityEngine;
using UnityEngine.UI;

public class UnitCellView : MonoBehaviour {

    [SerializeField] private Text nameText = null;
    [SerializeField] private Text hpText = null;
    [SerializeField] private Text raceStatusText = null;
    [SerializeField] private FieldSpriteImage sprite = null;

    public FieldSpriteImage Sprite { get => sprite; }

    public void Populate(Unit unit, PointerLayer pointers = null) {
        if (name != null) {
            nameText.text = unit.Name;
        }
        if (raceStatusText != null) {
            hpText.text = unit[StatTag.HP] + "/" + unit[StatTag.MHP];
            if (unit.Status == null || unit.IsDead) {
                raceStatusText.text = unit.SpeciesString;
            } else {
                raceStatusText.text = unit.Status.ToString();
            }
        } else if (hpText != null) {
            if (unit.Status == null || unit.IsDead) {
                hpText.text = unit[StatTag.HP] + "/" + unit[StatTag.MHP];
            } else {
                hpText.text = unit.Status.ToString() + "/" + unit[StatTag.MHP];
            }
            hpText.text = unit[StatTag.HP] + "/" + unit[StatTag.MHP];
        }
        sprite.Populate(unit);

        if (pointers != null && GetComponent<SelectableCell>()?.selectedState.GetComponent<PointerComponent>() != null) {
            GetComponent<SelectableCell>()?.selectedState.GetComponent<PointerComponent>().Populate(pointers);
        }
    }
}
