public class EffectEnemyTarget : AbilEffect {

    protected new EffectEnemyTargetData data;

    public EffectEnemyTarget(EffectEnemyTargetData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    public override bool IsBattleUsable() => true;
    public override bool IsMapUsable() => false;
}