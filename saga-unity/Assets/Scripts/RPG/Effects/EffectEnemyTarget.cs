using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class EffectEnemyTarget : AbilEffect {

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

    public override async Task ResolveAsync(Intent intent) {
        var battle = intent.Battle;
        var frontname = intent.Targets[0].Name;
        var itemname = intent.Item.Name;
        var username = intent.Actor.Name;
        var targets = new List<Unit>();
        switch (data.projector) {
            case OffenseProjectorType.SINGLE_ENEMY:
                if (!intent.IsRecursivelyTriggered) {
                    await battle.WriteLineAsync(username + " attacks " + frontname + " by " + itemname + ".");
                }
                targets.Add(intent.Targets[0]);
                break;
            case OffenseProjectorType.GROUP_ENEMY:
                if (!intent.IsRecursivelyTriggered) {
                    await battle.WriteLineAsync(username + " attacks " + frontname + " by " + itemname + ".");
                }
                targets.AddRange(intent.Targets);
                break;
            case OffenseProjectorType.ALL_ENEMY:
                if (!intent.IsRecursivelyTriggered) {
                    await battle.WriteLineAsync(username + " attacks by " + itemname + ".");
                }
                targets.AddRange(intent.Targets);
                break;
        }
        
        var power = CalcAttackPower(battle, intent.Actor);
        var roll = Random.Range(0.0f, 1.0f);
        foreach (var target in targets) {
            if (CheckIfHits(intent, target, power, roll)) {
                await ApplyEffect(intent, target, power);
            }
        }
        
        await FinishIntentResolution(intent);
    }

    protected abstract int CalcAttackPower(Intent intent);

    protected abstract bool CheckIfHits(Intent intent, Unit target, int power, float roll);

    protected abstract Task ApplyEffect(Intent intent, Unit target, int power);

    protected virtual Task FinishIntentResolution(Intent intent) {
        // default is nothing
        return Task.FromResult(0);
    }
}