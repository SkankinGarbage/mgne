using UnityEngine;

public class AbilMutation : Mutation {

    private int loseSlot;
    private CombatItem gainAbil, loseAbil;

    public AbilMutation(Unit unit, int loseSlot, CombatItem gainAbil) : base(unit) { 
        this.loseSlot = loseSlot;
        this.gainAbil = gainAbil;
        loseAbil = unit.Equipment[loseSlot];
    }

    public AbilMutation(Unit unit, int level) 
        : this(unit, Random.Range(0, 4), GenerateMutationAbility(unit, level))  {
    }

    public override string Description {
        get {
            if (loseAbil == null || loseAbil.Name.Length == 0) {
                return "acquire " + gainAbil.Name;
            } else {
                return "lose " + loseAbil.Name + ", acquire " + gainAbil.Name;
            }
        }
    }

    public override string Message {
        get {
            if (loseAbil == null || loseAbil.Name.Length == 0) {
                return unit.Name + " acquired " + gainAbil.Name;
            } else {
                return unit.Name + " lost " + loseAbil.Name + " and acquired " + gainAbil.Name;
            }
        }
    }

    public override Stat Stat => null;

    public override void Apply() {
        unit.Equipment.SetSlot(loseSlot, gainAbil, true);
    }

    private static CombatItem GenerateMutationAbility(Unit unit, int level) {
        var table = MutationManager.MutationsTable;
        while (true) {
            var index = Random.Range(0, table.abils.Length);
            var abilData = table.abils[index];

            var failed = false;
            foreach (var abil in unit.Equipment) {
                if (abil != null && abil.Data.key == abilData.abil.key) {
                    failed = true;
                    break;
                }
            }
            if (failed) continue;

            var roll = Random.Range(0, 100);
            var checksToPass = 1 + Mathf.Max(0, abilData.level - level);
            for (var i = 0; i < checksToPass; i += 1) {
                if (roll < (100 - abilData.chance)) {
                    failed = true;
                    break;
                }
            }
            if (failed) continue;

            return new CombatItem(abilData.abil);
        }
    }
}
