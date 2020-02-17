public class TempStats {

    public bool IsOneRoundOnly { get; private set; }

    private Unit target;
    private StatSet boost;

    public TempStats(Unit target, StatSet boost, bool oneRoundOnly = false) {
        this.target = target;
        this.boost = boost;
        IsOneRoundOnly = oneRoundOnly;
        target.Stats.AddSet(boost);
    }

    public void Decombine() {
        target.Stats.RemoveSet(boost);
    }
}
