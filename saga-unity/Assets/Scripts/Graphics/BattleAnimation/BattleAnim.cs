using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BattleAnim {

    protected BattleAnimData data;

    public BattleAnim(BattleAnimData data) {
        this.data = data;
    }

    public void PlaySound() {
        if (data.sound != null && data.sound.Length > 0) {
            Global.Instance().Audio.PlaySFX(data.sound);
        }
    }

    public async Task PlayAsync(BattleView view, List<Unit> targets) {
        PlaySound();
        await PlayInternalAsync(view, targets);
        view.framePool.DisposeAll();
    }

    protected IEnumerator RenderStepRoutine(BattleStepData data, BattleView view, List<Unit> targets, Vector2 offset, bool autohide = true) {
        float elapsed = 0.0f;
        List<AnimFrameSpriteComponent> frames = null;
        while (elapsed < data.start + data.duration) {
            elapsed += Time.deltaTime;
            if (elapsed >= data.start && frames == null) {
                frames = view.ShowBattleAnimationFrame(data, targets);
                foreach (var frame in frames) {
                    frame.ApplyOffset(offset);
                }
            }
            yield return null;
        }
        if (frames != null && autohide) {
            view.HideBattleAnimationFrames(frames);
        }
    }

    protected abstract Task PlayInternalAsync(BattleView view, List<Unit> targets);
}
