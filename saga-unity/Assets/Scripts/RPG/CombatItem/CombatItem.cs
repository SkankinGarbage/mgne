using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class CombatItem {

    public CombatItemData Data { get; protected set; }

    public Inventory Container { get; protected set; }
    public int UsesWhenAdded { get; protected set; }
    public int UsesRemaining { get; protected set; }
    public StatSet Stats { get; protected set; }

    public bool CanRestoreUses => Data.type == AbilityType.ABILITY;
    public int GoldValue => Data.cost * UsesRemaining / Data.uses;
    public bool IsBattleUseable => true; // TODO: effect.IsBattleUseable 
    public bool IsMapUseable => true;
    public StatSet RoboStats => Data.robostats;
    public string Name => UIUtils.GlyphifyString(Data.abilityName);

    public CombatItem(CombatItemData data) {
        Data = data;
        RestoreUses();
        Stats = new StatSet();
    }

    /// <param name="inventory">The inventory being added to, maybe null</param>
    public void OnAddedTo(Inventory inventory) {
        Container = inventory;
        UsesWhenAdded = UsesRemaining;
    }

    public void RestoreUses() {
        UsesRemaining = Data.uses;
    }

    public async Task OnMapUseDeferred(AbilMenuView menu) {
        // TODO:
        await Task.Delay(0);
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
