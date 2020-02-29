﻿using System.Threading.Tasks;

public class CombatItem {

    public CombatItemData Data { get; protected set; }

    public Inventory Container { get; protected set; }
    public int UsesWhenAdded { get; protected set; }
    public int UsesRemaining { get; protected set; }
    public StatSet Stats { get; protected set; }
    public AbilEffect Effect { get; protected set; }

    public bool CanRestoreUses => Data.type == AbilityType.ABILITY;
    public int GoldValue => Data.uses > 0 ? Data.cost * UsesRemaining / Data.uses : Data.cost;
    public bool IsBattleUseable => Effect.IsBattleUsable() && (UsesRemaining > 0 || Data.uses == 0);
    public bool IsMapUseable => true;
    public StatSet RoboStats => Data.robostats;
    public string Name => UIUtils.GlyphifyString(Data.abilityName);

    protected CombatItem() {
        Stats = new StatSet();
    }

    public CombatItem(CombatItemData data) : this() {
        Data = data;
        RestoreUses();
        Effect = AbilEffectFactory.CreateEffect(data.warhead, this);
    }

    public CombatItem(SerializedCombatItem data) : this() {
        Data = IndexDatabase.Instance().CombatItems.GetData(data.dataKey);
        UsesWhenAdded = data.usesWhenAdded;
        UsesRemaining = data.usesRemaining;
        Effect = AbilEffectFactory.CreateEffect(Data.warhead, this);
    }

    /// <param name="inventory">The inventory being added to, maybe null</param>
    public void OnAddedTo(Inventory inventory) {
        Container = inventory;
        UsesWhenAdded = UsesRemaining;
    }

    public void RestoreUses() {
        UsesRemaining = Data.uses;
    }

    public async Task UseOnMapAsync(AbilMenuView menu, Unit user) {
        await Effect.UseOnMapAsync(menu, user);
    }

    public void HalveUses() {
        UsesRemaining /= 2;
        DiscardIfNeeded();
    }

    public void DeductUse() {
        if (UsesRemaining > 0) {
            UsesRemaining -= 1;
        }
        DiscardIfNeeded();
    }

    public void DiscardIfNeeded() {
        if (UsesRemaining > 0) return;
        if (Data.uses == 0) return;
        if (Data.type == AbilityType.ABILITY) return;
        if (Container == null) return;

        Container.Drop(this);
    }
}
