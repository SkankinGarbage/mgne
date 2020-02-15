﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

public class Intent {

    public Unit Actor { get; private set; }
    public CombatItem Item { get; private set; }
    public List<Unit> Targets { get; private set; }
    public int Priority { get; private set; }
    public Battle Battle { get; private set; }

    public Intent(Unit actor, Battle battle) {
        Battle = battle;
        Actor = actor;
        Targets = new List<Unit>();
        Priority = actor[StatTag.AGI];
        if (Priority > 0) {
            Priority += Random.Range(0, Priority);
        }
    }

    public void SetItem(CombatItem itemToUse) {
        Item = itemToUse;
        Targets.Clear();
    }

    public override string ToString() {
        var actorname = Actor.Name;
        if (Item == null) {
            return actorname + " is undecided";
        } else {
            var itemname = Item.Name;
            if (Targets.Count > 0) {
                var targetname = Targets[0].Name;
                return actorname + " attacks " + targetname + "(s) by " + itemname;
            } else {
                return actorname + " uses " + itemname;
            }
        }
    }
    
    /// <returns>The index of the used item in the inventory of the actor</returns>
    public int FindIndexForItem() {
        if (Item == null) {
            // todo: history
            return 0;
        } else {
            return Actor.Equipment.SlotForItem(Item);
        }
    }

    public async Task ResolveAsync() {
        if (!Actor.CanAct) {
            return;
        }
        if (!Targets.Any(unit => unit.IsAlive) && !Item.Effect.CanTargetDead()) {
            await Battle.View.PrintDoesNothingRoutine(Actor);
            return;
        }

        await Item.Effect.ResolveAsync(this);
    }
}