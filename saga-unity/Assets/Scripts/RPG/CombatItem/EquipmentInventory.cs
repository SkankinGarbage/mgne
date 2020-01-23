using UnityEngine;
using System.Collections;

public class EquipmentInventory : Inventory {

    private const int Capacity = 8;

    public EquipmentInventory(Unit unit) : base(Capacity) {

    }

    /// <returns>True if at least one item can be used in battle</returns>
    public bool ContainsBattleUseableItems() {
        for (int slot = 0; slot < capacity; slot += 1) {
            var toCheck = items[slot];
            if (toCheck != null && toCheck.IsBattleUseable && toCheck.UsesRemaining > 0) {
                return true;
            }
        }
        return false;
    }
}
