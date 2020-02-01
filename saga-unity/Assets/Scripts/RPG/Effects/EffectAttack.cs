public class EffectAttack : EffectCombat {

    protected new EffectAttackData data;

    public EffectAttack(EffectAttackData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
}