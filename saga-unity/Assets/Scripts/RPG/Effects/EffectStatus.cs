public class EffectStatus : EffectEnemyTarget {

    protected new EffectStatusData data;

    public EffectStatus(EffectStatusData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
}