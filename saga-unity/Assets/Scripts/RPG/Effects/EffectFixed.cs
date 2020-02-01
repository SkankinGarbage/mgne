public class EffectFixed : EffectCombat {

    protected new EffectFixedData data;

    public EffectFixed(EffectFixedData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
}