using System.Collections.Generic;
using System.Threading.Tasks;

public class BattleAnimSeries : BattleAnim {

    protected new BattleAnimSeriesData data;

    public BattleAnimSeries(BattleAnimSeriesData data) : base(data) {
        this.data = data;
    }

    protected override Task PlayInternalAsync(BattleView view, List<Unit> targets) {
        throw new System.NotImplementedException();
    }
}
