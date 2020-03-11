using UnityEngine;
using UnityEngine.UI;

public class CollectableView : MonoBehaviour {

    [SerializeField] private Text countText = null;
    [SerializeField] private Text nameText = null;

    public void Populate(CollectableData collectable, int count) {
        nameText.text = UIUtils.GlyphifyString(collectable.displayName);
        countText.text = count.ToString();
    }
}
