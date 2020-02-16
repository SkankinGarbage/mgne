using System;

public class EffectDefend : EffectAllyTarget {

    protected new EffectDefendData data;

    public int ShieldDodgeBonus => data.shielding;

    public EffectDefend(EffectDefendData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
    
    public override bool IsMapUsable() => false;

    public DefendResult DefendAgainst(Intent attackIntent, Unit target, DamageType damType) {
        // TODO:
        return new DefendResult() {
            callback = null,
            countered = false,
        };
    }

    public class DefendResult {
        public Action<Intent, Unit> callback;
        public bool countered;
    }
}