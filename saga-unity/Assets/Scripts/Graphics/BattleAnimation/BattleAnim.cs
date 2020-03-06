using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class BattleAnim {

    protected BattleAnimData data;

    public BattleAnim(BattleAnimData data) {
        this.data = data;
    }

    public void PlaySound() {
        if (data.sound != null) {
            Global.Instance().Audio.PlaySFX(data.sound);
        }
    }

    public async Task PlayAsync(BattleView view, List<Unit> targets) {
        PlaySound();
        await PlayInternalAsync(view, targets);
    }

    protected abstract Task PlayInternalAsync(BattleView view, List<Unit> targets);
}
