using System.Collections.Generic;
using System.Threading.Tasks;

public class BattleAnimShots : BattleAnim {

    protected new BattleAnimShotsData data;

    public BattleAnimShots(BattleAnimShotsData data) : base(data) {
        this.data = data;
    }

    protected override Task PlayInternalAsync(BattleView view, List<Unit> targets) {
        throw new System.NotImplementedException();
    }
}
