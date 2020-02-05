using System.Collections.Generic;
using System.Linq;

public class EffectHeal : EffectAllyTarget {

    protected new EffectHealData data;

    public EffectHeal(EffectHealData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
    
    public override bool IsMapUsable() => true;

    protected override void ApplyMapEffect(Unit caster, IEnumerable<Unit> targets) {
        bool affected = false;
        foreach (var victim in targets) {
            var user = caster ?? victim;
            if (!victim.IsDead) {
                if (victim.Heal(CalcPower(user)) > 0) {
                    affected = true;
                }
            }
            var status = victim.Status;
            if (status != null && data.heals.Where(x => x == status.Data).Count() > 0) {
                status.Heal(victim);
                affected = true;
            }
            if (data.useRestore == UseRestoreType.RESTORES_USES) {
                victim.RestoreAbilityUses();
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
        if (data.powerStat != StatTag.MANA || user.BaseStats[StatTag.MANA] > 0) {
            // this prevents humans from healing a fixed amount with curative items despite 0 mana
            heal += data.@base;
        }
        return heal;
    }
}