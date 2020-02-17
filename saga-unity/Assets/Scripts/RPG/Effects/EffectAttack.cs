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

    protected override bool IsPhysical() {
        return data.defendStat == StatTag.DEF;
    }

    protected int CalculateDamage(Battle battle, Unit target, int power) {
        return ReducePowerByDefense(target, data.defendStat, power);
    }
}