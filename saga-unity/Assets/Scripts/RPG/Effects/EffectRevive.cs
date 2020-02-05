using System.Collections.Generic;

public class EffectRevive : EffectAllyTarget {

    protected new EffectReviveData data;

    public EffectRevive(EffectReviveData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
    
    public override bool IsMapUsable() => true;
    public override bool CanTargetDead() => true;

    protected override void ApplyMapEffect(Unit caster, IEnumerable<Unit> targets) {
        bool affected = false;
        foreach (Unit victim in targets) {
            if (victim[StatTag.HP] <= 0) {
                var user = caster ?? victim;
                victim.Heal(CalcPower(user));
                affected = true;
            }
        }
        FinishMapEffect(affected);
    }

    private int CalcPower(Unit user) {
        int heal = data.power;
        if (data.powerStat != null) {
            heal *= user[(StatTag)data.powerStat];
        }
        heal += data.@base;
        return heal;
    }
}