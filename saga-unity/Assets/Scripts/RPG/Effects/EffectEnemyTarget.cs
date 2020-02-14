using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class EffectEnemyTarget : AbilEffect {

    protected new EffectEnemyTargetData data;

    public EffectEnemyTarget(EffectEnemyTargetData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    public override bool IsBattleUsable() => true;
    public override bool IsMapUsable() => false;

    public override async Task<List<Unit>> AcquireTargetsAsync(Unit actor, Battle battle, bool useAI) {
        switch (data.projector) {
            case OffenseProjectorType.ALL_ENEMY:
                if (useAI) {
                    return new List<Unit>(battle.Player.Members);
                } else {
                    battle.View.enemySelector.SelectAll();
                    var confirmed = await battle.View.enemySelector.ConfirmSelectionAsync();
                    return confirmed ? new List<Unit>(battle.Enemy.Members) : null;
                }
            case OffenseProjectorType.SINGLE_ENEMY:
            case OffenseProjectorType.GROUP_ENEMY:
                if (useAI) {
                    return battle.Player.SelectFrontfacingGroup();
                } else {
                    var selection = await battle.View.enemySelector.SelectItemAsync();
                    return selection >= 0 ? battle.Enemy.Groups[selection] : null;
                }
        }
        return null;
    }
}