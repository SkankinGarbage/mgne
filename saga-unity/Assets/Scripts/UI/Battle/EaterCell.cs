using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SelectableCell))]
public class EaterCell : MonoBehaviour {

    [SerializeField] private FieldSpriteImage sprite = null;
    [SerializeField] private Text fromText = null;
    [SerializeField] private Text toText = null;

    public void Populate(Unit unit, CharaData toForm) {
        sprite.Populate(unit);
        if (unit.Race == Race.MONSTER && unit.IsAlive) {
            if (toForm == null) {
                fromText.text = "Nothing";
                toText.text = "happens   ";
            } else {
                fromText.text = unit.SpeciesString;
                toText.text = "to " + toForm.species;
            }
        } else {
            fromText.text = "Nothing";
            toText.text = "happens   ";
        }
        GetComponent<SelectableCell>().SetSelectable(unit.IsAlive);
    }
}
