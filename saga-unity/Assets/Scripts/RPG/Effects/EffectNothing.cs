public class EffectNothing : AbilEffect {

    protected new EffectNothingData data;

    public EffectNothing(EffectNothingData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    public override bool IsBattleUsable() => false;
    public override bool IsMapUsable() => false;
}