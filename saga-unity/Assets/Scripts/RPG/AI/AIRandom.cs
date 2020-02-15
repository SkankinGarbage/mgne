using UnityEngine;
using System.Collections;

public class AIRandom : AIBase {

    public AIRandom(Unit unit) : base(unit) {

    }

    protected override void PopulateIntent(Intent intent, Party allies, Party enemy) {
        var index = Random.Range(0, EquipmentInventory.Capacity);
        CombatItem item = null;
        var tries = 0;
        while ((item == null || !item.IsBattleUseable) && tries < EquipmentInventory.Capacity) {
            item = actor.Equipment[index];
            tries += 1;
            index += 1;
            if (index >= EquipmentInventory.Capacity) {
                index = 0;
            }
        }

        intent.SetItem(item);
        if (item != null) {
            item.Effect.AcquireTargetsAsync(actor, intent.Battle, true);
        }
    }
}
