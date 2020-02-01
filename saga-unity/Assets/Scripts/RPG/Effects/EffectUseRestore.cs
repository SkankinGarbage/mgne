public class EffectUseRestore : AbilEffect {

    protected new EffectUseRestoreData data;

    public EffectUseRestore(EffectUseRestoreData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    public override bool IsBattleUsable() => false;
    public override bool IsMapUsable() => true;
}