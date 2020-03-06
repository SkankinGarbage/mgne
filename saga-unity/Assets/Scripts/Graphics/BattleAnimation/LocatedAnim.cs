/// <summary>
/// A battle animation that plays at a given location. Data struct.
/// </summary>
public struct LocatedAnim {

    public BattleAnimStrip strip;
    public int x, y;

    public LocatedAnim(BattleAnimStrip strip) {
        this.strip = strip;
        x = 0;
        y = 0;
    }
}
