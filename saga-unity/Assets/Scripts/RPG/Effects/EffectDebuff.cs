public class EffectDebuff : EffectEnemyTarget {

    protected new EffectDebuffData data;

    public EffectDebuff(EffectDebuffData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
}