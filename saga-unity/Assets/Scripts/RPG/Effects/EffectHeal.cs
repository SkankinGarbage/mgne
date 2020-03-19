using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EffectHeal : EffectAllyTarget {

    protected new EffectHealData data;

    public EffectHeal(EffectHealData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
    
    public override bool IsMapUsable() => true;

    protected override bool ApplyMapEffect(Unit caster, IEnumerable<Unit> targets) {
        bool affected = false;
        foreach (var victim in targets) {
            var user = caster ?? victim;
            if (!victim.IsDead) {
                if (victim.Heal(CalculatePower(user)) > 0) {
                    affected = true;
                }
            }
            var status = victim.Status;
            if (status != null && data.heals.Where(x => x == status.Data).Count() > 0) {
                status.Heal(victim);
                affected = true;
            }
            if (data.useRestore == UseRestoreType.RESTORES_USES) {
                victim.RestoreAbilityUses();
                affected = true;
            }
        }
        FinishMapEffect(affected);
        return affected;
    }

    public override async Task ResolveAsync(Intent intent) {
        var battle = intent.Battle;
        var user = intent.Actor;
        var username = user.Name;
        var item = intent.Item;
        var itemname = item.Name;
        var tab = BattleBox.Tab;
        var targets = new List<Unit>();

        switch (data.projector) {
            case AllyProjectorType.ALLY_PARTY:
            case AllyProjectorType.PLAYER_PARTY_ENEMY_GROUP:
                targets.AddRange(intent.Targets);
                await battle.WriteLineAsync(username + " uses " + itemname + ".");
                break;
            case AllyProjectorType.SINGLE_ALLY:
            case AllyProjectorType.USER:
                var target = intent.Targets[0];
                var targetname = target.Name;
                targets.Add(target);
                if (target == user) {
                    await battle.WriteLineAsync(username + " uses " + itemname + ".");
                } else {
                    await battle.WriteLineAsync(username + " uses " + itemname + " on " + targetname + ".");
                }
                break;
        }

        await PlayBattleAnimAsync(intent);

        foreach (var target in targets) {
            var targetname = target.Name;
            var heal = target.Heal(CalculatePower(user));
            if (heal > 0) {
                await battle.WriteLineAsync(tab + targetname + " recovered " + heal + " HP.");
            }
            var status = target.Status;
            if (status != null && data.heals.Contains(status.Data)) {
                await status.HealAsync(battle, target);
            } else if (heal <= 0) {
                await battle.WriteLineAsync(tab + "Nothing happens.");
            }
        }
    }

    private int CalculatePower(Unit user) {
        int heal = data.power;
        if (data.powerStat != null) {
            heal *= user[(StatTag)data.powerStat];
        }
        if (data.powerStat != StatTag.MANA || user.BaseStats[StatTag.MANA] > 0) {
            // this prevents humans from healing a fixed amount with curative items despite 0 mana
            heal += data.@base;
        }
        return heal;
    }
}