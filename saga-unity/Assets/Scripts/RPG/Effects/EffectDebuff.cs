using System.Threading.Tasks;

public class EffectDebuff : EffectEnemyTarget {

    protected new EffectDebuffData data;

    public EffectDebuff(EffectDebuffData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    protected override async Task ApplyEffect(Intent intent, Unit target, int power) {
        var victimname = target.Name;
        var tab = BattleBox.Tab;
        var statName = Stat.Get(data.drainStat).NameLong;
        var battle = intent.Battle;

        var debuffMult = power / 100f;
        if (data.defendStat.HasValue) {
            debuffMult -= target[data.defendStat.Value] * 0.66f / 100f;
        }
        if (debuffMult > .5f) {
            debuffMult = .5f;
        }
        var finalStat = (int)(target[data.drainStat] * (1.0f - debuffMult));
        var debuffPower = target[data.drainStat] - finalStat;
        if (finalStat < 0) {
            debuffPower = target[data.drainStat];
        }
        if (debuffPower > 0) {
            var mod = new StatSet();
            mod[data.drainStat] = -1 * debuffPower;
            battle.AddBoost(target, mod);
            await battle.WriteLineAsync(tab + victimname + "'s " + statName + " is down by "  + debuffPower + ".");
        } else {
            await battle.WriteLineAsync("Nothing happens.");
        }
    }

    protected override int CalculateAttackPowerAsync(Intent intent) {
        int power = data.power;
        if (data.attackStat != null) {
            power += intent.Actor[data.attackStat.Value] / 2;
        }
        return power;
    }

    protected override bool CheckIfHits(Intent intent, Unit target, int power, float roll) {
        return true;
    }
}