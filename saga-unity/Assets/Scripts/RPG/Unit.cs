﻿using System.Linq;
using System.Threading.Tasks;

public class Unit {

    // careful, only guaranteed for NPCs, as heroes are dynamic
    protected CharaData data;
    protected string name;

    public StatSet Stats { get; private set; }
    public StatSet BaseStats { get; private set; }
    public Status Status { get; set; }
    public EquipmentInventory Equipment { get; private set; }
    public AIBase AI { get; private set; }
    public string FieldSpriteTag { get; private set; }

    public int this[StatTag tag] { get => (int) Stats[tag]; }

    public bool Is(StatTag flag) => Stats[flag] > 0;
    public bool IsCarryingItemType(CombatItemData data) => Equipment.ContainsItemType(data);
    public string Name => name?.Length > 0 ? name : data.name?.Length > 0 ? data.name : SpeciesString;
    public Race Race => data.race;
    public string SpeciesString => (data.species?.Length > 0 ? data.species : data.race.ToString()) + " " + data.gender.Label();
    public MonsterFamilyData MonsterFamily => Race == Race.MONSTER ? data.family : null;
    public string Portrait => data.portrait;
    public bool CanAct => IsAlive && (Status == null || !Status.PreventsIntentions) && Equipment.ContainsBattleUseableItems();
    public bool IsAlive => !IsDead;
    public bool IsConfused => Status != null && Status.Confuses;
    public bool IsWeakTo(DamageType type) => type.WeakFlags().Any(flag => Is(flag));
    public bool IsImmuneTo(DamageType type) => type.ImmuneFlags().Any(flag => Is(flag));
    public bool IsResistantTo(DamageType type) => type.ResistFlags().Any(flag => Is(flag));

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
        BaseStats = new StatSet(data.stats);
        Equipment = new EquipmentInventory(this, data);
        FieldSpriteTag = data.appearance;
        AI = new AIRandom(this);
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

    /// <summary>To be called from the inventory</summary>
    public void OnEquip(CombatItem item) {
        Stats += item.Stats;
        if (Race == Race.ROBOT) {
            item.HalveUses();
            Stats += item.RoboStats;
        }
    }

    /// <summary>To be called from the inventory</summary>
    public void OnUnequip(CombatItem item) {
        Stats -= item.Stats;
        if (Race == Race.ROBOT) {
            item.HalveUses();
            Stats -= item.RoboStats;
            Heal(0);
        }
    }

    public void PermanentlyModifyStat(StatTag stat, int delta) {
        BaseStats[stat] += delta;
        Stats[stat] += delta;
    }

    public LuaUnit GetLuaUnit(LuaContext context) {
        return new LuaUnit(this, context);
    }

    /// <summary>
    /// Inflicts damage from a combat source. Does not apply defense.
    /// </summary>
    /// <param name="damage">The raw damage to inflict</param>
    /// <param name="physical">True if the damage was from a physical source</param>
    /// <returns>True if any damage was actually taken</returns>
    public bool InflictDamage(int damage, bool physical) {
        Stats[StatTag.HP] -= damage;
        //if (mutantManager != null) {
        //    mutantManager.recordEvent(MutantEvent.DAMAGED);
        //    if (physical) {
        //        mutantManager.recordEvent(MutantEvent.DAMAGED_PHYSICALLY);
        //    }
        //}
        if (Stats[StatTag.HP] <= 0) {
            Stats[StatTag.HP] = 0;
            return true;
        }
        return false;
    }

    public async Task UpdateForEndOfRoundAsync(Battle battle) {
        if (IsDead) {
            return;
        }
        if (Status != null) {
            await Status.CheckHealAsync(battle, this);
            await Status?.UpdateForEndOfRoundAsync(battle, this);
        }
        if (Is(StatTag.REGENERATING)) {
            var toRegen = (int) (Stats[StatTag.MHP] / 10f);
            if (Stats[StatTag.HP] + toRegen > Stats[StatTag.MHP]) {
                toRegen = (int) (Stats[StatTag.MHP] - Stats[StatTag.HP]);
            }
            if (toRegen > 0) {
                await battle.WriteLineAsync(Name + " recovers by " + toRegen + ".");
                Heal(toRegen);
            }
        }
    }
}
