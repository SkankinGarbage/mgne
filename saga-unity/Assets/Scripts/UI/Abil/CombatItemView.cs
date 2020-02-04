using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CombatItemView : MonoBehaviour {

    [SerializeField] private Text nameLabel = null;
    [SerializeField] private Text usesLabel = null;

    public void Populate(CombatItem item) {
        nameLabel.text = item?.Name;
        usesLabel.text = item?.Data.uses > 0 ? item.UsesRemaining.ToString() : "";
    }
}
