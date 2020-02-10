using System.Collections.Generic;
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

    protected virtual void ApplyMapEffect(Unit caster, IEnumerable<Unit> targets) {
        Debug.LogError("Not implemented: ApplyMapEffect");
    }
}