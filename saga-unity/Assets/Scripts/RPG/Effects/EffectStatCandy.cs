public class EffectStatCandy : AbilEffect {

    protected new EffectStatCandyData data;

    public EffectStatCandy(EffectStatCandyData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    public override bool IsBattleUsable() => false;
    public override bool IsMapUsable() => true;
}