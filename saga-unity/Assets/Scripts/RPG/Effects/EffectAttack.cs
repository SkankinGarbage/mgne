using System.Threading.Tasks;
using UnityEngine;

public class EffectAttack : EffectCombat {

    protected new EffectAttackData data;

    public EffectAttack(EffectAttackData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    protected override Task<int> CalculateAttackPowerAsync(Intent intent) {
        return Task.FromResult(CalcStatPower(intent.Actor, data.attackStat, data.power));
    }

    protected override bool IsPhysical() {
        return data.defendStat == StatTag.DEF;
    }

    protected override Task<int> CalculateDamageAsync(Battle battle, int power, Unit target) {
        if (data.defendStat != StatTag.NONE && !target.IsWeakTo(data.damType)) {
            power -= target[data.defendStat];
        }
        return Task.FromResult(power);
    }

    protected override bool CheckCombatFormulaHit(Battle battle, Unit user, Unit target, float roll) {
        if (data.miss == MissType.ALWAYS_HITS) return true;
        int temp = 100 - (target[StatTag.AGI] + CalculateShieldDodgeBonus(battle, target) - user[StatTag.AGI]);
        float chance = temp / 100f;
        return roll < chance;
    }

    protected override Task FinishIntentResolution(Intent intent) {
        var manager = intent.Battle.GetMutationManagerForUnit(intent.Actor);
        switch (data.attackStat) {
            case StatTag.AGI:
                manager.RecordEvent(MutantEvent.USED_AGI);
                break;
            case StatTag.MANA:
                manager.RecordEvent(MutantEvent.USED_MANA);
                break;
            case StatTag.STR:
                manager.RecordEvent(MutantEvent.USED_STR);
                break;
            default:
                // doesn't matter
                break;
        }

        return base.FinishIntentResolution(intent);
    }
}