public class EffectBoost : EffectAllyTarget {

    protected new EffectBoostData data;

    public EffectBoost(EffectBoostData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
    
    public override bool IsMapUsable() => false;
}