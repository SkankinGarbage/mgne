using UnityEngine;
using UnityEngine.UI;

public class SaveInfoView : MonoBehaviour {

    [SerializeField] private ListView FieldSprites = null;
    [SerializeField] private Text LeaderName = null;
    [SerializeField] private Text Timestamp = null;
    [SerializeField] private Text LocationName = null;

    public void Populate(SaveInfoData info, SaveMenuView.Mode mode, int slot) {
        if (info == null) {
            GetComponent<SelectableCell>().SetSelectable(mode == SaveMenuView.Mode.Save);
            LeaderName.text = "Slot " + slot;
            Timestamp.text = null;
            LocationName.text = null;
            foreach (Transform cell in FieldSprites.GetCells()) {
                cell.GetComponent<FieldSpriteImage>().Populate((string) null);
            }
        } else {
            GetComponent<SelectableCell>().SetSelectable(true);
            LeaderName.text = info.LeaderName;
            Timestamp.text = UIUtils.FormatDateTime(UIUtils.TimestampToDateTime(info.Timestamp));
            LocationName.text = info.Location;
            FieldSprites.Populate(info.FieldSpriteTags, (cell, tag) => {
                cell.GetComponent<FieldSpriteImage>().Populate(tag);
            });
        }
    }
}
