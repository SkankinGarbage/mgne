public class EffectDefend : EffectAllyTarget {

    protected new EffectDefendData data;

    public EffectDefend(EffectDefendData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
    
    public override bool IsMapUsable() => false;
}