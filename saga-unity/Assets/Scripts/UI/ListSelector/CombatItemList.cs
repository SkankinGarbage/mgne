using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatItemList : ListView {

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
            view.Populate(item, pointers);
        });
    }
}
