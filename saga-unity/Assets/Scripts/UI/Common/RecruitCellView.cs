using UnityEngine;
using UnityEngine.UI;

public class RecruitCellView : MonoBehaviour {

    [SerializeField] private FieldSpriteImage sprite = null;
    [SerializeField] private Text recruitName = null;

    public void Populate(CharaData data) {
        if (data == null) {
            sprite.Populate((string) null);
            recruitName.text = "More information";
        } else {
            sprite.Populate(data.appearance);
            if (data.species == null || data.species.Length == 0) {
                recruitName.text = data.race.Name().ToUpper();
                while (recruitName.text.Length < 10) recruitName.text += " ";
                recruitName.text += data.gender.Label();
            } else {
                recruitName.text = data.species;
            }
        }
    }
}
