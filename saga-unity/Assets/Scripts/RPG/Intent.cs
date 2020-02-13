using System.Collections.Generic;
using UnityEngine;

public class Intent {

    public Unit Actor { get; private set; }
    public CombatItem Item { get; private set; }
    public List<Unit> Targets { get; private set; }
    public int Priority { get; private set; }

    protected Battle battle;

    public Intent(Unit actor, Battle battle) {
        this.battle = battle;
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
            return 0;
        } else {
            return Actor.Equipment.SlotForItem(Item);
        }
    }
}
