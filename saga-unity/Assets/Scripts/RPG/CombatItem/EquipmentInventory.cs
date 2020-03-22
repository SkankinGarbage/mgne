using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : Inventory {

    public const int DefaultCapacity = 8;

    private Unit owner;

    protected EquipmentInventory(Unit unit) : base(DefaultCapacity) {
        owner = unit;
    }

    public EquipmentInventory(Unit unit, CharaData data) : this(unit) {
        for (int i = 0; i < Capacity && i < data.equipped.Length; i += 1) {
            var itemData = data.equipped[i];
            if (itemData != null) {
                SetSlot(i, new CombatItem(itemData), firstEquip:true);
            }
        }
    }

    public EquipmentInventory(Unit unit, List<SerializedCombatItem> items) : this(unit) {
        for (var i = 0; i < items.Count; i += 1) {
            var item = items[i];
            if (item != null) {
                SetSlot(i, new CombatItem(item), firstEquip:false);
            }
        }
    }

    public override bool UsesRegenerateAt(int slot) {
        return base.UsesRegenerateAt(slot) || owner.Race == Race.ROBOT;
    }

    public override CombatItem SetSlot(int slot, CombatItem item, bool firstEquip) {
        var old = items[slot];
        if (old != null) owner.OnUnequip(old, firstEquip);
        if (item != null) owner.OnEquip(item, firstEquip);
        var result = base.SetSlot(slot, item, firstEquip);
        return result;
    }

    public override bool IsSlotReservedAt(int slot) {
        return 
            owner.Is(StatTag.EQUIPMENT_FIX) ||
            owner.Race == Race.MONSTER ||
            owner.Race == Race.MUTANT && slot <= 4;
    }

    /// <returns>True if at least one item can be used in battle</returns>
    public bool ContainsBattleUseableItems() {
        for (int slot = 0; slot < base.Capacity; slot += 1) {
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
        for (var i = 0; i < Capacity; i += 1) {
            var item = items[i];
            if (UsesRegenerateAt(i)) {
                item?.RestoreUses();
            }
        }
    }
}
