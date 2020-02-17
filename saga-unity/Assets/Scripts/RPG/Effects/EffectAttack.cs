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
        if (data.defendStat.HasValue && !target.IsWeakTo(data.damType)) {
            power -= target[data.defendStat.Value];
        }
        return Task.FromResult(power);
    }

    protected override bool CheckCombatFormulaHit(Battle battle, Unit user, Unit target, float roll) {
        if (data.miss == MissType.ALWAYS_HITS) return true;
        int temp = 100 - (target[StatTag.AGI] + CalculateShieldDodgeBonus(battle, target) - user[StatTag.AGI]);
        float chance = temp / 100f;
        return roll < chance;
    }
}