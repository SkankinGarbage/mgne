using System.Collections.Generic;
using System.Threading.Tasks;

public class BattleAnimSound : BattleAnim {

    protected new BattleAnimSoundData data;

    public BattleAnimSound(BattleAnimSoundData data) : base(data) {
        this.data = data;
    }

    protected override async Task PlayInternalAsync(BattleView view, List<Unit> targets) {
        await Task.Delay((int)(data.duration * 1000));
    }
}
