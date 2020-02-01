public class EffectHeal : EffectAllyTarget {

    protected new EffectHealData data;

    public EffectHeal(EffectHealData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
    
    public override bool IsMapUsable() => true;
}