using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleAnimSeries : BattleAnim {

    protected new BattleAnimSeriesData data;

    public BattleAnimSeries(BattleAnimSeriesData data) : base(data) {
        this.data = data;
    }

    protected override async Task PlayInternalAsync(BattleView view, List<Unit> targets) {
        var routines = new List<IEnumerator>();
        var duration = (float)data.count / data.concurrent;
        for (var i = 0; i < data.count; i += 1) {
            float start = i * duration / data.count;
            routines.Add(RenderStripRoutine(view, targets, start));
        }
        await CoUtils.RunParallel(routines.ToArray(), view);
    }

    private IEnumerator RenderStripRoutine(BattleView view, List<Unit> targets, float timeOffset) {
        yield return CoUtils.Wait(timeOffset);
        var routines = new List<IEnumerator>();
        var span = data.span / data.granularity;
        var offset = new Vector2(
            data.granularity * Random.Range(0, span + 1) - data.span / 2,
            data.granularity * Random.Range(0, span + 1) - data.span / 2);
        foreach (var step in data.anim.steps) {
            routines.Add(RenderStepRoutine(step, view, targets, offset));
        }
        yield return CoUtils.RunParallel(routines.ToArray(), view);
    }
}
