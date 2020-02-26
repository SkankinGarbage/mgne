using System.Threading.Tasks;
using UnityEngine;

public class UnitList : MonoBehaviour {

    [SerializeField] public DynamicListSelector selector = null;
    [SerializeField] private ListView partyCells = null;
    [SerializeField] private PointerLayer pointers = null;

    public void Populate() {
        partyCells.Populate(Global.Instance().Party, (obj, unit) => {
            var cell = obj.GetComponent<UnitCellView>();
            cell.Populate(unit, pointers);
        });
    }

    public async Task<Unit> SelectUnitAsync(Party party) {
        var slot = await selector.SelectItemAsync();
        if (slot == -1) return null;
        return party[slot];
    }

    public async Task<Unit> SelectUnitTargetAsync() {
        bool wasActive = gameObject.activeInHierarchy;
        gameObject.SetActive(true);
        var unit = await SelectUnitAsync(Global.Instance().Data.Party);
        gameObject.SetActive(wasActive);
        return unit;
    }
}