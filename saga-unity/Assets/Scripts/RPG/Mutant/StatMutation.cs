public class StatMutation : Mutation {

    private Stat stat;
    private StatTag tag;
    private readonly int delta;

    public StatMutation(Unit unit, StatTag tag, int delta = 2) : base(unit) {
        this.tag = tag;
        this.delta = delta;
        stat = Stat.Get(tag);
    }

    public override string Description => "raise " + tag.ToString();

    public override string Message => unit.Name + "'s " + stat.NameLong + " rose by " +  delta + ".";

    public override Stat Stat => stat;

    public override void Apply() {
        unit.PermanentlyModifyStat(tag, delta);
    }
}
