﻿using UnityEngine;
using System.Collections;

public class Unit {

    // careful, only guaranteed for NPCs, as heroes are dynamic
    protected CharaData data;

    public StatSet Stats { get; private set; }
    public Status Status { get; private set; }
    public EquipmentInventory Equipment { get; private set; }
    public string FieldSpriteTag { get; private set; }

    public float this[StatTag tag] { get => Stats[tag]; }

    public bool IsCarryingItemType(CombatItemData data) => Equipment.ContainsItemType(data);

    public bool IsDead {
        get {
            if (Stats[StatTag.HP] <= 0) {
                return true;
            }
            if (Status != null && Status.IsDeadly) {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Creates a new unit from serialized data. This should be the only point where units are
    /// created, as afterwise they'll be serialized and loaded up that way, or created on the fly
    /// for monsters
    /// </summary>
    public Unit(CharaData data) {
        this.data = data;
        Stats = new StatSet(data.stats);
        Equipment = new EquipmentInventory(this);
        FieldSpriteTag = data.appearance;
    }

    public void RestoreHP() {
        Heal((int) Stats[StatTag.MHP]);
    }

    public void RestoreAbilityUses() {
        Equipment.RestoreAbilityUses();
    }

    /// <returns>The actual amount healed</returns>
    public int Heal(int amount) {
        int old = (int) Stats[StatTag.HP];
        Stats[StatTag.HP] += amount;
        if (Stats[StatTag.HP] > Stats[StatTag.MHP]) {
            Stats[StatTag.HP] = Stats[StatTag.MHP];
        }
        return (int) Stats[StatTag.HP] - old;
    }
}