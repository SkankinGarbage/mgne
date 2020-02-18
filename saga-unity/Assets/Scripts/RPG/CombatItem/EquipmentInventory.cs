﻿using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : Inventory {

    public const int Capacity = 8;

    private Unit owner;

    public EquipmentInventory(Unit unit, CharaData data) : base(Capacity) {
        owner = unit;
        for (int i = 0; i < capacity && i < data.equipped.Length; i += 1) {
            var itemData = data.equipped[i];
            if (itemData != null) {
                SetSlot(i, new CombatItem(itemData));
            }
        }
    }

    public override CombatItem SetSlot(int slot, CombatItem item) {
        var old = items[slot];
        if (old != null) owner.OnUnequip(old);
        var result = base.SetSlot(slot, item);
        if (item != null) owner.OnEquip(item);
        return result;
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

    public CombatItem SelectRandomBattleItem() {
        var possible = new List<CombatItem>();
        foreach (var item in this) {
            if (item != null && item.IsBattleUseable) {
                possible.Add(item);
            }
        }
        if (possible.Count == 0) {
            return null;
        } else {
            return possible[Random.Range(0, possible.Count)];
        }
    }

    public void RestoreAbilityUses() {
        foreach (var item in items) {
            item.RestoreUses();
        }
    }
}
