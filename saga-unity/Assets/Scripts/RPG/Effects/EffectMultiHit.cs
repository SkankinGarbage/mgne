using System.Threading.Tasks;

public class EffectMultihit : EffectCombat {

    protected new EffectMultihitData data;

    protected Battle lastUsedBattle;
    protected int[] sequence;
    protected int seqIndex;

    public EffectMultihit(EffectMultihitData data, CombatItem item) : base(data, item) {
        this.data = data;

        var hitsStrings = data.hits.Split(',');
        sequence = new int[hitsStrings.Length];
        for (int i = 0; i < hitsStrings.Length; i += 1) {
            sequence[i] = int.Parse(hitsStrings[i]);
        }
    }

    protected override async Task<int> CalculateAttackPowerAsync(Intent intent) {
        var tab = BattleBox.Tab;
        if (lastUsedBattle == intent.Battle) {
            seqIndex += 1;
        } else {
            lastUsedBattle = intent.Battle;
            seqIndex = 0;
        }
        seqIndex = seqIndex % sequence.Length;
        var hits = sequence[seqIndex];
        await intent.Battle.WriteLineAsync(tab + "Hit " + hits + " times.");
        return CalcStatPower(intent.Actor, data.attackStat, data.power) * hits;
    }

    protected override Task<int> CalculateDamageAsync(Battle battle, int power, Unit target) {
        return Task.FromResult(ReducePowerByDefense(target, data.defendStat, power));
    }

    protected override bool CheckCombatFormulaHit(Battle battle, Unit user, Unit target, float roll) {
        foreach (var effect in battle.GetDefensesForUnit(target)) {
            if (effect.ShieldDodgeBonus > 100) {
                return false;
            }
        }
        return true;
    }

    protected override bool IsPhysical() {
        return data.defendStat == StatTag.DEF;
    }
}