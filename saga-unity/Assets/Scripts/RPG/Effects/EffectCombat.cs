public class EffectCombat : EffectEnemyTarget {

    protected new EffectCombatData data;

    public EffectCombat(EffectCombatData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
}