public class EffectRevive : EffectAllyTarget {

    protected new EffectReviveData data;

    public EffectRevive(EffectReviveData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
    
    public override bool IsMapUsable() => true;
    public override bool CanTargetDead() => true;
}