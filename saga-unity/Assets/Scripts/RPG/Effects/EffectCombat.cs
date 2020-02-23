using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class EffectCombat : EffectEnemyTarget {

    protected const float StunChance = 0.6f;

    protected new EffectCombatData data;

    public EffectCombat(EffectCombatData data, CombatItem item) : base(data, item) {
        this.data = data;
    }

    protected override async Task ApplyEffect(Intent intent, Unit target, int power) {
        var username = intent.Actor.Name;
        var itemname = intent.Item.Name;
        var victimname = target.Name;
        var tab = BattleBox.Tab;

        // Collect list of all effects triggered by this attack
        var callbacks = new List<Action<Intent, Unit>>();
        bool countered = false;
        foreach (var defense in intent.Battle.GetDefensesForUnit(target)) {
            var result = defense.DefendAgainst(intent, target, data.damType);
            if (result.callback != null) {
                callbacks.Add(result.callback);
            }
            if (result.countered) {
                countered = true;
                break;
            }
        }

        if (countered) {
            // This attack has been blocked
            // no need to print a message because the block effect will cover it
        } else if (IsTargetWeak(target) && HasFlag(OffenseFlag.CRITICAL_ON_WEAKNESS)) {
            // This attack insta-killed the victim
            target.InflictDamage(intent.Battle, target[StatTag.MHP], false);
            await intent.Battle.WriteLineAsync("");
            await intent.Battle.WriteLineAsync("");
            await intent.Battle.WriteLineAsync(tab + tab + "CRITICAL HIT!");
            await intent.Battle.WriteLineAsync("");
            await intent.Battle.WriteLineAsync(victimname + " is dead.");
            await intent.Battle.CheckDeathAsync(target, true);
        } else {
            // This attack may have damaged the victim
            var damage = await CalculateAttackPowerAsync(intent);
            if (!HasFlag(OffenseFlag.IGNORE_RESISTANCES)) {
                if (target.IsImmuneTo(data.damType)) {
                    await intent.Battle.WriteLineAsync(tab + victimname + " is immune to " + itemname + ".");
                    damage = 0;
                } else if (target.IsResistantTo(data.damType)) {
                    await intent.Battle.WriteLineAsync(tab + victimname + " is resistant to " + itemname + ".");
                    damage /= 2;
                }
            }
            if (damage > 0) {
                await intent.Battle.View.PlayBackDamageRoutine(target, damage);
                target.InflictDamage(intent.Battle, damage, IsPhysical());
                await intent.Battle.CheckDeathAsync(target);
                if (HasFlag(OffenseFlag.DRAIN_LIFE) && !target.Is(StatTag.UNDEAD)) {
                    int healed = intent.Actor.Heal(damage);
                    if (healed > 0) {
                        await intent.Battle.WriteLineAsync(tab + username + " recovers " + damage + " HP.");
                    }
                }
                if (HasFlag(OffenseFlag.STUNS_ON_HIT) && target.IsAlive) {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) < StunChance && intent.Battle.CancelAction(target)) {
                        await intent.Battle.WriteLineAsync(tab + "A stunning blow!");
                    }
                }
            } else {
                await intent.Battle.View.PlayBackDamageRoutine(target, 0);
            }
        }
        
        foreach (var callback in callbacks) {
            callback(intent, target);
        }
    }

    protected override async Task<bool> CheckHitAsync(Intent intent, Unit target, int power, float roll) {
        var tab = BattleBox.Tab;
        var username = intent.Actor.Name;
        var victimname = target.Name;

        if (HasFlag(OffenseFlag.ONLY_AFFECT_UNDEAD) && !target.Is(StatTag.UNDEAD)) {
            // Enemy is exempt
            await intent.Battle.WriteLineAsync(tab + "Nothing happens.");
            return false;

        } else if (HasFlag(OffenseFlag.ONLY_AFFECT_HUMANS) && target.Race != Race.HUMAN) {
            // Enemy is exempt
            await intent.Battle.WriteLineAsync(tab + "Nothing happens.");
            return false;

        } else if (!CheckCombatFormulaHit(intent.Battle, intent.Actor, target, roll)) {
            // We missed... why?
            if (CalculateShieldDodgeBonus(intent.Battle, target) > 0) {
                var defenses = intent.Battle.GetDefensesForUnit(target);
                var blocker = defenses.FirstOrDefault();
                var shield = blocker.Item;
                await shield.Effect.OnBlock(intent.Battle, target);
            } else {
                await intent.Battle.WriteLineAsync(tab + username + " misses " + victimname + ".");
            }
            return false;

        } else {
            // We actually hit the jerk
            return true;
        }
    }

    protected int CalcStatPower(Unit actor, StatTag? powerStat, int power) {
        int temp = power;
        int result = 0;
        if (powerStat.HasValue) {
            temp *= Mathf.RoundToInt(actor[powerStat.Value] / 4f);
            result = actor[powerStat.Value];
        }
        if (temp > 2) {
            result += (temp + UnityEngine.Random.Range(0, temp / 2));
        }
        return result;
    }

    protected bool IsTargetWeak(Unit target) {
        if (target.IsWeakTo(data.damType)) {
            return true;
        }
        if (target.MonsterFamily != null) {
            foreach (var family in data.slayerFamiles) {
                if (target.MonsterFamily == family) {
                    return true;
                }
            }
        }
        return false;
    }

    protected int ReducePowerByDefense(Unit target, StatTag? defendStat, int power) {
        if (defendStat.HasValue && !IsTargetWeak(target)) {
            int reduction = target[defendStat.Value];
            if (defendStat == StatTag.DEF) {
                reduction = Mathf.FloorToInt(Mathf.Pow(reduction, 1.15f));
            }
            power -= reduction;
        }
        return power;
    }

    protected int CalculateShieldDodgeBonus(Battle battle, Unit defender) {
        var shielding = 0;
        foreach (var defense in battle.GetDefensesForUnit(defender)) {
            shielding += defense.ShieldDodgeBonus;
        }
        return shielding;
    }

    protected bool HasFlag(OffenseFlag flag) {
        return data.sideEffects.Contains(flag);
    }

    /// <summary>Only relevant for mutant stuff</summary>
    protected abstract bool IsPhysical();

    protected abstract Task<int> CalculateDamageAsync(Battle battle, int power, Unit target);

    protected abstract bool CheckCombatFormulaHit(Battle battle, Unit user, Unit target, float roll);
}