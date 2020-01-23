using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Inventory {

    protected int capacity;

    protected CombatItem[] items;
    private List<CombatItem> Items {
        get {
            // warning: will contain nulls
            return new List<CombatItem>(items);
        }
    }
    public CombatItem this[int slot] {
        get {
            return items[slot];
        }
        private set {
            items[slot] = value;
        }
    }

    public Inventory(int capacity) {
        this.capacity = capacity;
        items = new CombatItem[capacity];
    }

    /// <summary>Reserved slots are mutant/monster abils and can't be filled normally</summary>
    public virtual bool IsSlotReservedAt(int slot) {
        return false;
    }

    /// <summary>Sets a combat item in the given inventory slot</summary>
    /// <returns>The item that used to be there, or null</returns>
    public CombatItem SetSlot(int slot, CombatItem item) {
        if (slot >= capacity) {
            Debug.LogWarningFormat("Out of bounds inventory check " + slot);
            return null;
        } else {
            CombatItem old = this[slot];
            items[slot] = item;
            if (item != null) {
                item.OnAddedTo(this);
            }
            if (old != null) {
                old.OnAddedTo(null);
            }
            return old;
        }
    }

    /// <returns>True if that item at that slot can regenerate its uses</returns>
    public bool UsesRegenerateAt(int slot) {
        return Items[slot] != null && Items[slot].RegeneratesUses;
    }

    /// <returns>The gold worth of the item in that slot, 0 for unsellable/unbuyable</returns>
    public int GoldValueAt(int slot) {
        return Items[slot] == null ? 0 : Items[slot].GoldValue;
    }

    /// <returns>The slot containing this exact item, or -1 for doesn't contain</returns>
    public int SlotForItem(CombatItem item) {
        if (item == null) return -1;
        for (int i = 0; i < capacity; i += 1) {
            if (Items[i] == item) {
                return i;
            }
        }
        return -1;
    }

    /// <returns>The first index containing an item built off the given data</returns>
    public int SlotForItemType(CombatItemData data) {
        if (data == null) return -1;
        for (int i = 0; i < capacity; i += 1) {
            if (Items[i].Data == data) {
                return i;
            }
        }
        return -1;
    }

    public bool IsFull() {
        for (int i = 0; i < capacity; i += 1) {
            if (items[i] == null && !IsSlotReservedAt(i)) {
                return false;
            }
        }
        return true;
    }

    /// <returns>The index the item was added at, or -1 if failed to do so</returns>
    public int Add(CombatItem item) {
        for (int i = 0; i < capacity; i += 1) {
            if (Items[i] == null && !IsSlotReservedAt(i)) {
                Items[i] = item;
                return i;
            }
        }
        return -1;
    }

    /// <param name="item">The item to drop, assuming it's in the inventory</param>
    public void Drop(CombatItem item) {
        Drop(SlotForItem(item));
    }

    /// <param name="item">The slot to drop from</param>
    public void Drop(int slot) {
        items[slot] = null;
    }

    /// <returns>True if any item is based off of that data type</returns>
    public bool ContainsItemType(CombatItemData itemData) {
        return SlotForItemType(itemData) >= 0;
    }
    
    /// <returns>True if a drop was performed</returns>
    public bool DropItemType(CombatItemData itemData) {
        var slot = SlotForItemType(itemData);
        if (slot < 0) return false;
        Drop(slot);
        return true;
    }

    /// <summary>Swaps the items at the given slots</summary>
    public void Swap(int slot1, int slot2) {
        CombatItem item1 = items[slot1];
        CombatItem item2 = items[slot2];
        Drop(slot1);
        Drop(slot2);
        Items[slot1] = item2;
        Items[slot2] = item1;
    }
}
