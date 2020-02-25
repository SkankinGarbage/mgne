using System.Threading.Tasks;
using UnityEngine;

public class Status {

    public StatusData Data { get; private set; }

    public bool IsDeadly => Data.lethality == LethalityType.DEADLY || Data.lethality == LethalityType.DEATH;
    public bool PreventsIntentions => IsDeadly || Data.preventChance > 0 || Confuses;
    public bool Confuses => Data.randomChance > 0;
    public override string ToString() => Data.fullName;

    public Status(StatusData data) {
        Data = data;
    }

    public async Task InflictAsync(Battle battle, Unit target) {
        if (battle != null) {
            var tab = BattleBox.Tab;
            await battle.WriteLineAsync(tab + target.Name + Data.inflictString + ".");
        }
        if (Data.lethality == LethalityType.DEATH) {
            target.InflictDamage(battle, target[StatTag.HP], false);
        } else {
            target.Status = this;
        }
    }

    /// <summary>Remove the status off the victim, out of battle</summary>
    public void Heal(Unit victim) {
        if (Data.lethality == LethalityType.DEATH) {
            victim.Heal(1);
        }
        victim.Status = null;
    }

    public async Task CheckHealAsync(Battle battle, Unit unit) {
        if (Random.Range(0, 100) < Data.recoverChance) {
            await HealAsync(battle, unit);
        }
    }

    public async Task HealAsync(Battle battle, Unit unit) {
        await battle.WriteLineAsync("");
        await battle.WriteLineAsync(unit.Name + " " + Data.healString + ".");
        Heal(unit);
    }

    public async Task UpdateForEndOfRoundAsync(Battle battle, Unit unit) {
        if (Data.dot > 0) {
            var dot = Data.dot;
            if (Data.dotStat.HasValue) {
                dot += unit[Data.dotStat.Value] / 10;
            }
            unit.InflictDamage(battle, dot, false);
            await battle.WriteLineAsync(unit.Name + Data.inflictString + ", " + unit.Name + " takes " + dot + " damage.");
            await battle.CheckDeathAsync(unit);
        } else if (Data.preventChance > 0) {
            await battle.WriteLineAsync(unit.Name + Data.inflictString + ".");
        }
    }

    public void UpdateForEndOfCombat(Unit unit) {
        if (Data.recover == RecoverType.RECOVER_AFTER_BATTLE) {
            unit.Status = null;
        }
    }
}
