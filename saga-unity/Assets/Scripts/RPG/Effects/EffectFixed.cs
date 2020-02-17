using UnityEngine;

public class EffectFixed : EffectCombat {

    protected new EffectFixedData data;

    public EffectFixed(EffectFixedData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    protected override int CalcAttackPower(Intent intent) {
        if (data.range > 0) {
            return data.@base + Random.Range(0, data.range);
        } else {
            return data.@base;
        }
    }

    protected override bool IsPhysical() {
        return data.damType == DamageType.PHYSICAL;
    }
}
