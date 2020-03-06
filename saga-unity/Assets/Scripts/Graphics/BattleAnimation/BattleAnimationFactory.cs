using UnityEngine;

public static class BattleAnimationFactory {

    public static BattleAnim Create(BattleAnimData data) {
        if (data is BattleAnimSeriesData) {
            return new BattleAnimSeries((BattleAnimSeriesData)data);
        } else if (data is BattleAnimShaderData) {
            return new BattleAnimShader((BattleAnimShaderData)data);
        } else if (data is BattleAnimShotsData) {
            return new BattleAnimShots((BattleAnimShotsData)data);
        } else if (data is BattleAnimSoundData) {
            return new BattleAnimSound((BattleAnimSoundData)data);
        } else if (data is BattleAnimStripData) {
            return new BattleAnimStrip((BattleAnimStripData)data);
        } else {
            Debug.LogError("Unknown battle anim subclass " + data.GetType());
            return null;
        }
    }
}
