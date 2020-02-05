using UnityEngine;
using UnityEngine.UI;

public class CombatItemView : MonoBehaviour {

    [SerializeField] private Text nameLabel = null;
    [SerializeField] private Text usesLabel = null;
    [SerializeField] private PointerLayer pointers = null;
    [SerializeField] private PointerComponent ownedPointer = null;

    public void Populate(CombatItem item, PointerLayer pointers = null) {
        nameLabel.text = item?.Name;
        usesLabel.text = item?.Data.uses > 0 ? item.UsesRemaining.ToString() : "";
        this.pointers = pointers != null ? pointers : this.pointers;
        ownedPointer.Populate(this.pointers);

        if (item != null) {
            gameObject.name = item.Name;
        }
    }
}
