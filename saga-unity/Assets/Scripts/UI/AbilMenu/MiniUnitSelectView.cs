using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MiniUnitSelectView : MonoBehaviour {

    [SerializeField] private DynamicListSelector selector = null;
    [SerializeField] private ListView partyCells = null;
    [SerializeField] private PointerLayer pointers = null;

    public void Populate() {
        partyCells.Populate(Global.Instance().Party, (obj, unit) => {
            var cell = obj.GetComponent<MiniUnitCellView>();
            cell.Populate(unit, pointers);
        });
    }

    public async Task<Unit> SelectUnitAsync(Party party) {
        var slot = await selector.SelectItemAsync();
        if (slot == -1) return null;
        return party[slot];
    }
}