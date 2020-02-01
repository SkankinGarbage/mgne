public class EffectTransform : AbilEffect {

    protected new EffectTransformData data;

    public EffectTransform(EffectTransformData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    public override bool IsBattleUsable() => false;
    public override bool IsMapUsable() => true;
}