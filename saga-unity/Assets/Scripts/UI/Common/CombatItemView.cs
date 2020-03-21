using UnityEngine;
using UnityEngine.UI;

public class CombatItemView : MonoBehaviour {

    [SerializeField] private Text nameLabel = null;
    [SerializeField] private Text usesLabel = null;
    [SerializeField] private Text costLabel = null;
    [SerializeField] private PointerLayer pointers = null;
    [SerializeField] private PointerComponent ownedPointer = null;

    [SerializeField] private bool allowSelectNull = true;

    public CombatItem Item { get; private set; }

    public void Populate(CombatItem item, PointerLayer pointers = null) {
        Item = item;
        nameLabel.text = item?.Name;
        usesLabel.text = item?.Data.uses > 0 ? item.UsesRemaining.ToString() : (item == null ? "" : "-");
        this.pointers = pointers != null ? pointers : this.pointers;
        if (ownedPointer != null) {
            ownedPointer.Populate(this.pointers);
        }

        if (item != null) {
            gameObject.name = item.Name;
        }

        if (costLabel != null) {
            if (item == null ) {
                costLabel.text = "";
            } else {
                var goldValue = item.GoldValue;
                costLabel.text = goldValue > 0 ? goldValue.ToString() : "";
            }
        }

        GetComponent<SelectableCell>().SetSelectable(item != null || allowSelectNull);
    }
}
