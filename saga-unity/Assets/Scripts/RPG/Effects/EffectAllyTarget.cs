using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class EffectAllyTarget : AbilEffect {

    protected new EffectAllyTargetData data;

    public EffectAllyTarget(EffectAllyTargetData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    public override bool IsBattleUsable() => true;

    public override async Task UseOnMapAsync(IItemUseableMenu menu, Unit user) {
        bool wasActive, confirmed;
        switch (data.projector) {
            case AllyProjectorType.ALLY_PARTY:
            case AllyProjectorType.PLAYER_PARTY_ENEMY_GROUP:
                menu.SelectAll();
                wasActive = menu.IsActive();
                menu.SetActive(true);
                confirmed = await menu.ConfirmSelectionAsync();
                menu.SetActive(wasActive);
                if (confirmed) {
                    ApplyMapEffect(user, Global.Instance().Data.Party);
                }
                item.DeductUse();
                break;
            case AllyProjectorType.SINGLE_ALLY:
                var target = await menu.SelectUnitTargetAsync();
                if (target != null) {
                    ApplyMapEffect(user, new Unit[] { target });
                }
                break;
            case AllyProjectorType.USER:
                ApplyMapEffect(user, new Unit[] { user });
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

    public override List<Unit> AcquireRandomTargets(Unit actor, Battle battle) {
        switch (data.projector) {
            case AllyProjectorType.PLAYER_PARTY_ENEMY_GROUP:
                var possibleGroups = new List<List<Unit>>();
                possibleGroups.Add(new List<Unit>(battle.Player));
                possibleGroups.AddRange(battle.Enemy.Groups);
                return possibleGroups[Random.Range(0, possibleGroups.Count)];
            case AllyProjectorType.ALLY_PARTY:
                return new List<Unit>(Random.Range(0.0f, 1.0f) > 0.5f ? battle.Player.Members : battle.Enemy.Members);
            case AllyProjectorType.SINGLE_ALLY:
                var possibleTargets = new List<Unit>();
                possibleTargets.AddRange(battle.Enemy);
                possibleTargets.AddRange(battle.Player);
                return new List<Unit>() { possibleTargets[Random.Range(0, possibleTargets.Count)] };
            case AllyProjectorType.USER:
                return new List<Unit>() { actor };
        }
        return null;
    }

    protected virtual void ApplyMapEffect(Unit caster, IEnumerable<Unit> targets) {
        Debug.LogError("Not implemented: ApplyMapEffect");
    }
}