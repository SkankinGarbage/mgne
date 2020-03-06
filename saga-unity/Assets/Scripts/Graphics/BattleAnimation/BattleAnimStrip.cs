using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class BattleAnimStrip : BattleAnim {

    protected new BattleAnimStripData data;

    public BattleAnimStrip(BattleAnimStripData data) : base(data) {
        this.data = data;
    }

    protected override async Task PlayInternalAsync(BattleView view, List<Unit> targets) {
        await RenderRoutine();
    }

    private IEnumerator RenderRoutine(BattleStepData step, BattleView view, List<Unit> targets) {
        float elapsed = 0.0f;
        for (var finished = 0; finished < data.steps.Length; ) {
            foreach (var step in data.steps) {
                if (elapsed > step.duration + step.start) {
                    finished += 1;
                    HideStepIfNeeded(step, view, targets);
                } else if (elapsed > step.start) {
                    RenderStepIfNeeded(step, view, targets);
                }
            }
            yield return null;
        }
    }

    private void RenderStepIfNeeded(BattleStepData step, BattleView view, List<Unit> targets) {
        view
    }

    private void HideStepIfNeeded(BattleStepData step, BattleView view, List<Unit> targets) {

    }
}
