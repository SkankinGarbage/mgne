using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;

public class EquipMenuView : MonoBehaviour {

    private const string PrefabPath = "Prefabs/UI/Equip/EquipMenu";

    [SerializeField] private MainMenuCellView unitCell = null;
    [SerializeField] private List<StatLabelView> statLabels = null;

    [SerializeField] private CombatItemList equipList = null;
    [SerializeField] private CombatItemList inventoryList = null;

    public static EquipMenuView ShowDefault() {
        var menu = Instantiate(Resources.Load<GameObject>(PrefabPath)).GetComponent<EquipMenuView>();
        menu.transform.SetParent(Global.Instance().UI.transform, worldPositionStays: false);
        return menu;
    }

    public void Populate(Unit unit) {
        unitCell.Populate(unit);
        foreach (var label in statLabels) {
            label.Populate(unit.Stats);
        }
        equipList.Populate(unit.Equipment);
        inventoryList.Populate(Global.Instance().Data.Inventory);
    }

    public async Task DoMenuAsync(Unit unit) {
        var inventory = Global.Instance().Data.Inventory;
        while (true) {
            Populate(unit);

            var equipSlot = await equipList.Selector.SelectItemAsync(null, true);
            if (equipSlot == -1) {
                break;
            }
            if (unit.Equipment.IsSlotReservedAt(equipSlot)) {
                AudioManager.PlayFail();
                continue;
            }

            var inventorySlot = await inventoryList.Selector.SelectItemAsync();
            if (inventorySlot == -1) {
                continue;
            }

            var oldOccupant = unit.Equipment.Drop(equipSlot);
            unit.Equipment.SetSlot(equipSlot, inventory.Drop(inventorySlot));
            inventory.SetSlot(inventorySlot, oldOccupant);
        }
    }

    public IEnumerator CloseRoutine() {
        Destroy(gameObject);
        yield break; ;
    }
}
