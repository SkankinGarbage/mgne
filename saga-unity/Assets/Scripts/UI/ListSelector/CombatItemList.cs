using UnityEngine;
using System.Collections.Generic;

public class CombatItemList : ListView {

    [SerializeField] private bool includeNonCombatItems = true;
    [SerializeField] private PointerLayer pointers = null;

    private GenericSelector selector;
    public GenericSelector Selector {
        get {
            if (selector == null) {
                selector = GetComponent<GenericSelector>();
            }
            return selector;
        }
    }

    public void Populate(IEnumerable<CombatItem> items) {
        Populate(items, (obj, item) => {
            var view = obj.GetComponent<CombatItemView>();
            if (!includeNonCombatItems && item != null && !item.IsBattleUseable) {
                view.Populate(null, pointers);
            } else {
                view.Populate(item, pointers);
            }
        });
    }

    public void Populate(IEnumerable<CombatItemData> datas) {
        Populate(datas, (obj, itemData) => {
            var view = obj.GetComponent<CombatItemView>();
            view.Populate(new CombatItem(itemData), pointers);
        });
    }
}
