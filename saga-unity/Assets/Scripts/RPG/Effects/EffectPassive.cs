public class EffectPassive : AbilEffect {

    protected new EffectPassiveData data;

    public EffectPassive(EffectPassiveData data, CombatItem item) : base(data, item) {
        this.data = data;
        item.Stats.AddSet(data.stats);
    }

    public override bool IsBattleUsable() => false;
    public override bool IsMapUsable() => false;
}