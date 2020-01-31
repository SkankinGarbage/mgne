using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public abstract class CombatItem {

    public CombatItemData Data { get; protected set; }

    public Inventory Container { get; protected set; }
    public int UsesWhenAdded { get; protected set; }
    public int UsesRemaining { get; protected set; }

    public bool CanRestoreUses => Data.type == AbilityType.ABILITY;
    public int GoldValue => Data.cost * UsesRemaining / Data.uses;
    public bool IsBattleUseable => true; // TODO: effect.IsBattleUseable 
    public bool IsMapUseable => true;

    public CombatItem(CombatItemData data) {
        Data = data;
        RestoreUses();
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
}
