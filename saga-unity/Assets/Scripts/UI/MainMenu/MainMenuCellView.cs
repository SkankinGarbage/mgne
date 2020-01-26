using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuCellView : MonoBehaviour {

    [SerializeField] private Text nameText = null;
    [SerializeField] private Text hpText = null;
    [SerializeField] private Text raceStatusText = null;
    [SerializeField] private FieldSpriteImage sprite = null;

    public void Populate(Unit unit) {
        nameText.text = unit.Name;
        hpText.text = unit[StatTag.HP] + "/" + unit[StatTag.MHP];
        raceStatusText.text = unit.Status == null ? unit.SpeciesString : unit.Status.ToString();
        sprite.Populate(unit.FieldSpriteTag);
    }
}
