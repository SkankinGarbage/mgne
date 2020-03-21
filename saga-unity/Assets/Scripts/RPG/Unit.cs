using System.Linq;
using System.Threading.Tasks;

public class Unit {

    private const int DefaultMeatChance = 38;

    // serialized
    protected CharaData data; // not guaranteed for heroes
    protected string name;
    public StatSet BaseStats { get; private set; }
    public Status Status { get; set; }
    public EquipmentInventory Equipment { get; private set; }

    // transient
    public StatSet Stats { get; private set; }
    public AIBase AI { get; private set; }
    public int LastCombatSlot { get; set; }

    public int this[StatTag tag] { get => (int) Stats[tag]; }

    public MonsterFamily MonsterFamily => Race == Race.MONSTER ? new MonsterFamily(data.family) : null;
    public Race Race => data.race;
    public string DataKey => data?.key;
    public string FieldSpriteTag => data.appearance;
    public string SpeciesString => data.species?.Length > 0 ? data.species : (data.race.Name() + " " + data.gender.Label());
    public string Portrait => data.portrait;
    public string Name => name?.Length > 0 ? name : data.name?.Length > 0 ? data.name : SpeciesString;
    public void SetName(string name) => this.name = name;
    public int GP => data.gp;
    public int MeatLevel => data.meatEatLevel;
    public int MeatDropChance => data.meatDropChance > 0 ? data.meatDropChance : (MonsterFamily != null ? DefaultMeatChance : 0);
    public bool Is(StatTag flag) => Stats[flag] > 0;
    public bool IsCarryingItemType(CombatItemData data) => Equipment.ContainsItemType(data);
    public bool CanAct => IsAlive && (Status == null || !Status.PreventsIntentions) && Equipment.ContainsBattleUseableItems();
    public bool IsAlive => !IsDead;
    public bool IsConfused => Status != null && Status.Confuses;
    public bool IsWeakTo(DamageType type) => type.WeakFlags().Any(flag => Is(flag));
    public bool IsImmuneTo(DamageType type) => type.ImmuneFlags().Any(flag => Is(flag));
    public bool IsResistantTo(DamageType type) => type.ResistFlags().Any(flag => Is(flag));
    public bool IsResistantTo(Status status) => Stats[status.Data.resistFlag] > 0;

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
    public Unit() {
        AI = new AIRandom(this);
    }

    public Unit(CharaData data) : this() {
        this.data = data;
        BaseStats = new StatSet(data.stats);
        Stats = new StatSet(data.stats);
        Equipment = new EquipmentInventory(this, data);
        RestoreHP();
    }

    public Unit(SerializedUnit serialized) : this() {
        if (serialized.dataKey?.Length > 0) {
            data = IndexDatabase.Instance().Units.GetData(serialized.dataKey);
        }
        Stats = new StatSet(serialized.currentStats ?? serialized.baseStats);
        BaseStats = new StatSet(serialized.baseStats);
        Equipment = new EquipmentInventory(this, serialized.equipment);
        SetName(serialized.name);
        
        if (serialized.statusKey?.Length > 0) {
            Status = new Status(IndexDatabase.Instance().Statuses.GetData(serialized.statusKey));
        }
    }

    public Unit Copy() {
        return new Unit(new SerializedUnit(this));
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
    public void OnEquip(CombatItem item, bool firstEquip) {
        if (firstEquip) {
            Stats += item.Stats;
            if (Race == Race.ROBOT) {
                item.HalveUses();
                Stats += item.RoboStats;
            }
        }
    }

    /// <summary>To be called from the inventory</summary>
    public void OnUnequip(CombatItem item, bool firstEquip) {
        Stats -= item.Stats;
        if (Race == Race.ROBOT) {
            Stats -= item.RoboStats;
            Heal(0);
        }
    }

    public void PermanentlyModifyStat(StatTag stat, int delta) {
        BaseStats[stat] += delta;
        Stats[stat] += delta;
    }

    public void TransformInto(CharaData newForm) {
        // destroy old stuff
        foreach (var item in Equipment) {
            if (item != null) {
                Equipment.Drop(item);
            }
        }

        // copy relevant stuff
        data = newForm;

        // create new stuff
        BaseStats = new StatSet(newForm.stats);
        Stats = new StatSet(newForm.stats);
        Heal((int) Stats[StatTag.MHP]);
        Status = null;
        Equipment = new EquipmentInventory(this, newForm);
        Global.Instance().Maps.Avatar.UpdateAppearance();
    }

    public LuaUnit GetLuaUnit(LuaContext context) {
        return new LuaUnit(this, context);
    }

    /// <summary>
    /// Inflicts damage from a combat source. Does not apply defense.
    /// </summary>
    /// <param name="battle">The battle context in which this occurs, or null</param>
    /// <param name="damage">The raw damage to inflict</param>
    /// <param name="physical">True if the damage was from a physical source</param>
    /// <returns>True if any damage was actually taken</returns>
    public bool InflictDamage(Battle battle, int damage, bool physical) {
        Stats[StatTag.HP] -= damage;
        if (battle != null) {
            var manager = battle.GetMutationManagerForUnit(this);
            manager.RecordEvent(MutantEvent.DAMAGED);
            if (physical) {
                manager.RecordEvent(MutantEvent.DAMAGED_PHYSICALLY);
            }
        }
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
            if (Status != null) {
                await Status.UpdateForEndOfRoundAsync(battle, this);
            }
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

    public async Task TryApplyStatusAsync(Status status, Battle battle) {
        var tab = BattleBox.Tab;
        if (status.Data.lethality == LethalityType.DEATH) {
            await battle.WriteLineAsync(tab + Name + status.Data.inflictString + ".");
            InflictDamage(battle, (int) Stats[StatTag.MHP], false);
        } else {
            if (Status == null || Status.Data.priority < status.Data.priority) {
                await battle.WriteLineAsync(tab + Name + status.Data.inflictString + ".");
                Status = status;
            }
        }
    }
}
