public class EffectTeleport : AbilEffect {

    protected new EffectTeleportData data;

    public EffectTeleport(EffectTeleportData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    public override bool IsBattleUsable() => false;
    public override bool IsMapUsable() => true;
}