using UnityEngine;
using UnityEngine.UI;

public class ReorderCellView : MonoBehaviour {

    [SerializeField] private UnitCellView unitCell = null;
    [SerializeField] private Text ordinalText = null;
    [SerializeField] private Text descriptionText = null;

    public FieldSpriteImage Sprite { get => unitCell.Sprite; }

    public void Populate(Unit unit, int ordinal) {
        switch (ordinal) {
            case 0:
                ordinalText.text = "1st";
                descriptionText.text = "Targeted 3x";
                break;
            case 1:
                ordinalText.text = "2nd";
                descriptionText.text = "Targeted 2x";
                break;
            case 2:
                ordinalText.text = "3rd";
                descriptionText.text = "Targeted 1x";
                break;
            case 3:
                ordinalText.text = "4th";
                descriptionText.text = "Targeted 1x";
                break;
            case 4:
                ordinalText.text = "5th";
                descriptionText.text = "Targeted 1x";
                break;
        }
        unitCell.Populate(unit);
    }
}
