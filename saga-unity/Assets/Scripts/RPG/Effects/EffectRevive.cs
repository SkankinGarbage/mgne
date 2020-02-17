using System.Collections.Generic;
using System.Threading.Tasks;

public class EffectRevive : EffectAllyTarget {

    protected new EffectReviveData data;

    public EffectRevive(EffectReviveData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
    
    public override bool IsMapUsable() => true;
    public override bool CanTargetDead() => true;

    protected override void ApplyMapEffect(Unit caster, IEnumerable<Unit> targets) {
        bool affected = false;
        foreach (Unit victim in targets) {
            if (victim[StatTag.HP] <= 0) {
                var user = caster ?? victim;
                victim.Heal(CalculatePower(user));
                affected = true;
            }
        }
        FinishMapEffect(affected);
    }

    private int CalculatePower(Unit user) {
        int heal = data.power;
        if (data.powerStat != null) {
            heal *= user[(StatTag)data.powerStat];
        }
        heal += data.@base;
        return heal;
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

        var affected = false;
        foreach (var victim in targets) {
            if (victim[StatTag.HP] <= 0) {
                var victimname = victim.Name;
                await battle.WriteLineAsync(tab + victimname + " is back to life.");
                victim.Heal(CalculatePower(user));
                affected = true;
            }
        }
        if (!affected) {
            await battle.WriteLineAsync(tab + "Nothing happens.");
        }
    }
}