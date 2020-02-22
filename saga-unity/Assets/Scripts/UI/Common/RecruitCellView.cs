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
            recruitName.text = data.species;
            switch (data.gender) {
                case Gender.MALE:
                    recruitName.text += " M";
                    break;
                case Gender.FEMALE:
                    recruitName.text += " F";
                    break;
                case Gender.NONE:
                    break;
            }
        }
    }
}
