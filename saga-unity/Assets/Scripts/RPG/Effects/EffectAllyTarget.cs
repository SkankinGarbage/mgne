using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class EffectAllyTarget : AbilEffect {

    protected new EffectAllyTargetData data;

    public EffectAllyTarget(EffectAllyTargetData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    public override bool IsBattleUsable() => true;

    public override async void OnMapUse(UnitList menu, Unit user) {
        switch (data.projector) {
            case AllyProjectorType.ALLY_PARTY:
            case AllyProjectorType.PLAYER_PARTY_ENEMY_GROUP:
                ApplyMapEffect(user, Global.Instance().Data.Party);
                break;
            case AllyProjectorType.SINGLE_ALLY:
                var target = await menu.SelectUnitTargetAsync();
                if (target != null) {
                    ApplyMapEffect(user, new Unit[] { target });
                }
                break;
            case AllyProjectorType.USER:
                if (user != null) {
                    ApplyMapEffect(user, new Unit[] { user });
                }
                break;
        }
    }

    public override async Task<List<Unit>> AcquireTargetsAsync(Unit actor, Battle battle, bool useAI) {
        switch (data.projector) {
            case AllyProjectorType.PLAYER_PARTY_ENEMY_GROUP:
                if (useAI) {
                    // uhh pretend you didn't see this
                    goto case AllyProjectorType.SINGLE_ALLY;
                } else {
                    // fallthrough
                    goto case AllyProjectorType.ALLY_PARTY;
                }
            case AllyProjectorType.ALLY_PARTY:
                if (useAI) {
                    return new List<Unit>(battle.Enemy.Members);
                } else {
                    battle.View.PopulateForAllySelection();
                    battle.View.allyList.selector.SelectAll();
                    var confirmed = await battle.View.allyList.selector.ConfirmSelectionAsync();
                    return confirmed ? new List<Unit>(battle.Player.Members) : null;
                }
            case AllyProjectorType.SINGLE_ALLY:
                if (useAI) {
                    int groupIndex = battle.Enemy.IndexFor(actor);
                    return battle.Enemy.Groups[groupIndex];
                } else {
                    battle.View.PopulateForAllySelection();
                    var selection = await battle.View.allyList.SelectUnitTargetAsync();
                    return selection == null ? null : new List<Unit>() { selection } ;
                }
            case AllyProjectorType.USER:
                if (useAI) {
                    return new List<Unit>() { actor };
                } else {
                    battle.View.PopulateForAllySelection();
                    battle.View.allyList.selector.Selection = battle.Player.IndexFor(actor);
                    var confirmed = await battle.View.allyList.selector.ConfirmSelectionAsync();
                    return confirmed ? new List<Unit>() { actor } : null;
                }
        }
        return null;
    }

    protected virtual void ApplyMapEffect(Unit caster, IEnumerable<Unit> targets) {
        Debug.LogError("Not implemented: ApplyMapEffect");
    }
}