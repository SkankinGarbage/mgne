public class TempStats {

    private Unit target;
    private StatSet boost;

    public TempStats(Unit target, StatSet boost) {
        this.target = target;
        this.boost = boost;
        target.Stats.AddSet(boost);
    }

    public void Decombine() {
        target.Stats.RemoveSet(boost);
    }
}
