using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

public class Intent {

    public Unit Actor { get; private set; }
    public CombatItem Item { get; private set; }
    public List<Unit> Targets { get; set; }
    public int Priority { get; private set; }
    public Battle Battle { get; private set; }
    public bool IsRecursivelyTriggered { get; private set; }
    public bool IsFinished { get; private set; }

    public Intent(Unit actor, Battle battle, bool isRecursivelyTriggered = false) {
        Battle = battle;
        Actor = actor;
        Targets = new List<Unit>();
        Priority = actor[StatTag.AGI];
        IsRecursivelyTriggered = isRecursivelyTriggered;
        if (Priority > 0) {
            Priority += Random.Range(0, Priority);
        }
    }

    public void SetItem(CombatItem itemToUse) {
        Item = itemToUse;
        Targets.Clear();
    }

    public void OnRoundStart() {
        Item.Effect.OnRoundStart(this);
    }

    public async Task Randomize() {
        await Battle.WriteLineAsync(Actor.Name + " is confused.");
        Item = Actor.Equipment.SelectRandomBattleItem();
        Item.Effect.AcquireRandomTargets(Actor, Battle);
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
        if (Actor.IsConfused) {
            await Randomize();
        }
        if (!Targets.Any(unit => unit.IsAlive) && !Item.Effect.CanTargetDead()) {
            await Battle.View.PrintDoesNothingRoutine(Actor);
            return;
        }

        await Item.Effect.ResolveAsync(this);

        IsFinished = true;
    }
}
