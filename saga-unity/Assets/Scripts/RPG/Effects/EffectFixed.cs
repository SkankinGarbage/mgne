using System.Threading.Tasks;
using UnityEngine;

public class EffectFixed : EffectCombat {

    protected new EffectFixedData data;

    public EffectFixed(EffectFixedData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    protected override Task<int> CalculateAttackPowerAsync(Intent intent) {
        if (data.range > 0) {
            return Task.FromResult(data.@base + Random.Range(0, data.range));
        } else {
            return Task.FromResult(data.@base);
        }
    }

    protected override Task<int> CalculateDamageAsync(Battle battle, int power, Unit target) {
        return Task.FromResult(ReducePowerByDefense(target, data.defenseStat, power));
    }

    protected override bool CheckCombatFormulaHit(Battle battle, Unit user, Unit target, float roll) {
        var temp = 100 - (data.accuracy - CalculateShieldDodgeBonus(battle, target));
        if (data.accStat.HasValue) {
            temp -= user[data.accStat.Value];
        }
        if (data.dodgeStat.HasValue) {
            temp += target[data.dodgeStat.Value];
        }
        float chance = temp / 100f;
        return roll > chance;
    }

    protected override bool IsPhysical() {
        return data.defenseStat == StatTag.DEF;
    }
}
