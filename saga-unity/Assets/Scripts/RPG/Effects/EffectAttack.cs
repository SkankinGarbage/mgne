using System.Threading.Tasks;
using UnityEngine;

public class EffectAttack : EffectCombat {

    protected new EffectAttackData data;

    public EffectAttack(EffectAttackData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    protected override int CalcAttackPower(Intent intent) {
        return CalcStatPower(intent.Actor, data.attackStat, data.power);
    }

    protected override bool CheckIfHits(Intent intent, Unit target, int power, float roll) {
        if (data.miss == MissType.ALWAYS_HITS) return true;
        int temp = 100 - (target[StatTag.AGI] + CalculateShieldDodgeBonus(intent.Battle, target) - intent.Actor[StatTag.AGI]);
        float chance = temp / 100f;
        return roll < chance;
    }

    protected override bool IsPhysical() {
        return data.defendStat == StatTag.DEF;
    }

    protected int CalculateDamage(Battle battle, Unit target, int power) {
        return ReducePowerByDefense(target, data.defendStat, power);
    }
}