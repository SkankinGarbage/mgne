using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleAnimStrip : BattleAnim {

    protected new BattleAnimStripData data;

    public BattleAnimStrip(BattleAnimStripData data) : base(data) {
        this.data = data;
    }

    protected override async Task PlayInternalAsync(BattleView view, List<Unit> targets) {
        await RenderRoutine(view, targets);
    }

    private IEnumerator RenderRoutine(BattleView view, List<Unit> targets) {
        var routines = new List<IEnumerator>();
        foreach (var step in data.steps) {
            routines.Add(RenderStepRoutine(step, view, targets, new Vector2(0, 0)));
        }
        yield return CoUtils.RunParallel(routines.ToArray(), view);
    }
}
