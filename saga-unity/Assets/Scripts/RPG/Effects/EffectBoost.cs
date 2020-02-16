using System.Collections.Generic;
using System.Threading.Tasks;

public class EffectBoost : EffectAllyTarget {

    protected new EffectBoostData data;

    public EffectBoost(EffectBoostData data, CombatItem item) : base(data, item) {
        this.data = data;
    }
    
    public override bool IsMapUsable() => false;

    public override async Task ResolveAsync(Intent intent) {
        var battle = intent.Battle;
        var user = intent.Actor;
        var username = user.Name;
        var stat = Stat.Get(data.stat);
        var statname = stat.NameLong;
        var itemname = intent.Item.Name;
        var tab = BattleBox.Tab;
        var targets = intent.Targets;
        int power = data.power;

        if (data.powerStat.HasValue) {
            power += user[data.powerStat.Value];
        }
        await battle.WriteLineAsync(username + " uses " + itemname + ".");
        foreach (var victim in targets) {
            var victimname = victim.Name;
            var boost = power;
            bool limited = false;
            if (data.cap != 0 && victim[stat.Tag] + boost > data.cap) {
                boost = data.cap - victim[stat.Tag];
                limited = true;
            }
            var mod = new StatSet();
            mod[stat.Tag] = boost;
            battle.AddBoost(victim, mod);
            if (boost > 0) {
                if (limited) {
                    await battle.WriteLineAsync(tab + victimname + "'s " + statname + " is up to " + data.cap + ".");
                } else {
                    await battle.WriteLineAsync(tab + victimname + "'s " + statname + " is up by " + boost + ".");
                }
            } else {
                await battle.WriteLineAsync(tab + "Nothing happens.");
            }
        }
    }
}