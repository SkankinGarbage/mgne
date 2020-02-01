public abstract class EffectAllyTarget : AbilEffect {

    protected new EffectAllyTargetData data;

    public EffectAllyTarget(EffectAllyTargetData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    public override bool IsBattleUsable() => true;
}