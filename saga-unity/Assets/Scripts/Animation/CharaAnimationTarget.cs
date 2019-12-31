using UnityEngine;
using System.Collections;
using MoonSharp.Interpreter;

[MoonSharpUserData]
[DisallowMultipleComponent]
public class CharaAnimationTarget : AnimationTarget {

    public enum Type {
        Attacker,
        Defender,
    }

    public Type type;
    public BattleAnimationPlayer player;
    public CharaEvent chara { get { return transform.parent.GetComponent<CharaEvent>(); } }

    private Vector3 originalDollPos;

    [MoonSharpHidden]
    public void ConfigureToBattler(BattleEvent battler) {
        chara.spritesheet = battler.GetComponent<CharaEvent>().spritesheet;
    }

    [MoonSharpHidden]
    public void PrepareForBattleAnimation(BattleAnimationPlayer player, Type type) {
        this.player = player;
        this.type = type;
        originalDollPos = transform.position;
    }

    [MoonSharpHidden]
    public override void ResetAfterAnimation() {
        transform.position = originalDollPos;

        foreach (SpriteRenderer renderer in renderers) {
            if (renderer.GetComponent<AfterimageComponent>()) {
                renderer.GetComponent<AfterimageComponent>().enabled = false;
            }
        }
    }

    // === COMMAND HELPERS =========================================================================


    private Vector3 CalculateJumpOffset(Vector3 startPos, Vector3 endPos) {
        Vector3 dir = (endPos - startPos).normalized;
        return endPos - 1.15f * dir;
    }

    private IEnumerator JumpRoutine(Vector3 endPos, float duration, float height) {
        Vector3 startPos = transform.position;
        float elapsed = 0.0f;
        while (transform.position != endPos) {
            elapsed += Time.deltaTime;
            Vector3 lerped = Vector3.Lerp(startPos, endPos, elapsed / duration);
            transform.position = new Vector3(
                    lerped.x,
                    lerped.y + ((elapsed >= duration)
                            ? 0
                            : Mathf.Sin(elapsed / duration * Mathf.PI) * height),
                    lerped.z);
            yield return null;
        }
    }

    // === LUA FUNCTIONS ===========================================================================

    // afterimage({enable?, count?, duration?});
    public void afterimage(DynValue args) {
        foreach (SpriteRenderer renderer in renderers) {
            AfterimageComponent imager = renderer.GetComponent<AfterimageComponent>();
            if (imager != null) {
                if (EnabledArg(args)) {
                    float imageDuration = FloatArg(args, ArgDuration, 0.05f);
                    int count = (int)FloatArg(args, ArgCount, 3);
                    imager.enabled = true;
                    imager.afterimageCount = count;
                    imager.afterimageDuration = imageDuration;
                } else {
                    imager.enabled = false;
                }
            }
        }
    }

    // strike({power? duration?})
    public void strike(DynValue args) { CSRun(cs_strike(args), args); }
    private IEnumerator cs_strike(DynValue args) {
        float elapsed = 0.0f;
        float duration = FloatArg(args, ArgDuration, 0.4f);
        float power = FloatArg(args, ArgPower, 0.1f);
        Vector3 startPos = transform.localPosition;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            transform.localPosition = new Vector3(
                    startPos.x + Random.Range(-power, power),
                    startPos.y,
                    startPos.z);
            yield return null;
        }
        transform.localPosition = startPos;
    }
}