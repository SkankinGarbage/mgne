public class EffectMultihit : EffectCombat {

    protected new EffectMultihitData data;

    public EffectMultihit(EffectMultihitData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
}