public abstract class Mutation {

    protected Unit unit;

    public Mutation(Unit unit) {
        this.unit = unit;
    }

    public abstract string Description { get; }
    public abstract string Message { get; }
    public abstract Stat Stat { get; }

    public abstract void Apply();

}
