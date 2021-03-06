﻿using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EquipMenuView : FullScreenMenuView {

    [SerializeField] private UnitCellView unitCell = null;
    [SerializeField] private List<StatLabelView> statLabels = null;

    [SerializeField] private CombatItemList equipList = null;
    [SerializeField] private CombatItemList inventoryList = null;

    public static EquipMenuView ShowDefault() {
        return Instantiate<EquipMenuView>("Prefabs/UI/Equip/EquipMenu");
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
            if (equipSlot < 0) {
                break;
            }
            if (unit.Equipment.IsSlotReservedAt(equipSlot)) {
                AudioManager.PlayFail();
                continue;
            }

            var inventorySlot = await inventoryList.Selector.SelectItemAsync();
            if (inventorySlot < 0) {
                continue;
            }

            if (unit.Race == Race.ROBOT && inventory[inventorySlot] != null && inventory[inventorySlot].UsesRemaining == 1) {
                AudioManager.PlayFail();
                continue;
            }

            var oldOccupant = unit.Equipment.Drop(equipSlot);
            unit.Equipment.SetSlot(equipSlot, inventory.Drop(inventorySlot), true);
            inventory.SetSlot(inventorySlot, oldOccupant, true);
        }
    }
}
