using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleAnimShots : BattleAnim {

    protected new BattleAnimShotsData data;

    private int finished;

    public BattleAnimShots(BattleAnimShotsData data) : base(data) {
        this.data = data;
    }

    protected override async Task PlayInternalAsync(BattleView view, List<Unit> targets) {
        await RenderRoutine(view, targets);
    }

    private IEnumerator RenderRoutine(BattleView view, List<Unit> targets) {
        var gainX = Random.Range(0, data.gainX * 2) - data.gainX;
        var gainY = Random.Range(0, data.gainY * 2) - data.gainY;
        var mirrorHoriz = Random.Range(0, 2) == 0;
        var mirrorVert = Random.Range(0, 2) == 0;

        var sprite = AnimFrameSpriteComponent.SpriteForStep(data.anim.steps[0]);
        var rows = Mathf.CeilToInt((float)data.count / data.cols);
        var width = (int)(data.cols * (sprite.bounds.size.x + data.padX) + (rows * gainX));
        var height = (int)(rows * (sprite.bounds.size.y + data.padY) + (data.cols * gainY));
        var startX = -1 * width / 2;
        var startY = -1 * height / 2;
        
        for (var i = 0; i < data.count; i += 1) {
            int col = i % data.cols;
            int row = (i - col) / rows;
            var offX = (int)(startX + col * (sprite.bounds.size.x + data.padX) +
                    (gainX * row) +
                    (Random.Range(0, 1.0f) * data.jitterX * 2) - data.jitterX);
            var offY = (int)(startY + row * (sprite.bounds.size.y + data.padY) +
                        (gainY * col) +
                        (Random.Range(0, 1.0f) * data.jitterY * 2) - data.jitterY);
            if (mirrorHoriz) {
                offX *= -1;
            }
            if (mirrorVert) {
                offY *= -1;
            }
            view.StartCoroutine(RenderStripRoutine(view, targets, new Vector2(offX, offY)));
            yield return CoUtils.Wait(data.delay);
        }
        while (finished < data.count) {
            yield return null;
        }
    }

    private IEnumerator RenderStripRoutine(BattleView view, List<Unit> targets, Vector2 offset) {
        var routines = new List<IEnumerator>();
        foreach (var step in data.anim.steps) {
            routines.Add(RenderStepRoutine(step, view, targets, offset, step != data.anim.steps[data.anim.steps.Length - 1]));
        }
        yield return CoUtils.RunParallel(routines.ToArray(), view);
        finished += 1;
    }
}
