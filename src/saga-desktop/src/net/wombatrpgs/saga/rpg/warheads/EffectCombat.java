/**
 *  EffectCombat.java
 *  Created on Apr 25, 2014 11:30:32 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.warheads;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.maps.MapThing;
import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.rpg.battle.Battle;
import net.wombatrpgs.saga.rpg.battle.Intent;
import net.wombatrpgs.saga.rpg.chara.Chara;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.saga.rpg.warheads.EffectDefend.DefendResult;
import net.wombatrpgs.saga.rpg.warheads.EffectDefend.PostDefend;
import net.wombatrpgs.sagaschema.rpg.abil.CombatItemMDO;
import net.wombatrpgs.sagaschema.rpg.abil.data.OffenseFlag;
import net.wombatrpgs.sagaschema.rpg.chara.data.Race;
import net.wombatrpgs.sagaschema.rpg.stats.Flag;
import net.wombatrpgs.sagaschema.rpg.stats.Stat;
import net.wombatrpgs.sagaschema.rpg.warheads.EffectCombatMDO;
import net.wombatrpgs.sagaschema.rpg.warheads.EffectDefendMDO;

/**
 * Superclass for some common combat ability effects. Specifically, the ones
 * that involve hitting people in the face.
 */
public abstract class EffectCombat extends EffectEnemyTarget {
	
	protected static final float STUN_CHANCE = .6f;
	
	protected EffectCombatMDO mdo;
	protected List<EffectDefend> riderEffects;

	/**
	 * Inherited constructor.
	 * @param	mdo				The data to create from
	 * @param	item			The item to create for
	 */
	public EffectCombat(EffectCombatMDO mdo, CombatItem item) {
		super(mdo, item);
		if (mdo.slayerFamiles == null) mdo.slayerFamiles = new String[0];
		this.mdo = mdo;
		
		riderEffects = new ArrayList<EffectDefend>();
		if (mdo.riders != null) {
			for (EffectDefendMDO riderMDO : mdo.riders) {
				if (!MapThing.mdoHasProperty(riderMDO.defendName)) {
					riderMDO.defendName = item.getName();
				}
				EffectDefend rider = new EffectDefend(riderMDO);
				assets.add(rider);
				
				CombatItemMDO newMDO = new CombatItemMDO();
				newMDO.abilityName = item.getName();
				newMDO.uses = 0;
				new CombatItem(newMDO, rider);
				
				riderEffects.add(rider);
			}
		}
	}
	
	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.EffectEnemyTarget#onAffect
	 * (net.wombatrpgs.saga.rpg.battle.Battle, Intent,
	 * net.wombatrpgs.saga.rpg.chara.Chara, net.wombatrpgs.saga.rpg.chara.Chara, int)
	 */
	@Override
	protected final void onAffect(Battle battle, Intent intent, Chara user, Chara victim, int power) {
		String username = user.getName();
		String itemname = intent.getItem().getName();
		String victimname = victim.getName();
		String tab = SConstants.TAB;
		
		if (victim.get(Stat.HP) < 0) {
			MGlobal.reporter.warn("Attacking a dead guy");
		}
		
		if (user.get(Stat.HP) < 0) {
			MGlobal.reporter.warn("Dead man walking!");
		}
		
		// Collect list of all effects triggered by this attack
		List<PostDefend> callbacks = new ArrayList<PostDefend>();
		boolean countered = false;
		for (EffectDefend defense : battle.getDefenses(victim)) {
			DefendResult result = defense.onAttack(victim, intent, mdo.damType);
			if (result.callback != null) {
				callbacks.add(result.callback);
			}
			if (result.countered) {
				countered = true;
				break;
			}
		}
		
		if (countered) {
			// This attack has been blocked
			// no need to print a message because the block effect will cover it
		} else if (weak(victim) && effect(OffenseFlag.CRITICAL_ON_WEAKNESS)) {
			// This attack insta-killed the victim
			victim.damage(victim.get(Stat.MHP), false);
			battle.println("");
			battle.println("");
			battle.println(tab + tab + "CRITICAL HIT!");
			battle.println("");
			battle.println(victimname + " is dead.");
			battle.checkDeath(victim, true);
		} else {
			// This attack may have damaged the victim
			int damage = calcDamage(battle, power, victim);
			if (!effect(OffenseFlag.IGNORE_RESISTANCES)) {
				if (isImmune(victim)) {
					battle.println(tab + victimname + " is resistant to " + itemname + ".");
					damage = 0;
				} else if (resists(victim)) {
					battle.println(tab + victimname + " is resistant to " + itemname + ".");
					damage /= 2;
				}
				
			}
			if (damage > 0) {
				battle.damagePlayback(victim, damage);
				victim.damage(damage, isPhysical());
				battle.checkDeath(victim, false);
				if (effect(OffenseFlag.DRAIN_LIFE) && !victim.is(Flag.UNDEAD)) {
					int healed = user.heal(damage);
					if (healed > 0) {
						battle.println(tab + username + " recovers " + damage + " HP.");
					}
				}
				if (effect(OffenseFlag.STUNS_ON_HIT) && victim.isAlive()) {
					if (MGlobal.rand.nextFloat() < STUN_CHANCE && battle.cancelAction(victim)) {
						battle.println(tab + "A stunning hit!");
					}
				}
			} else {
				battle.damagePlayback(victim, 0);
			}
		}
		
		// Battle damage calculations have been resolved, now callbacks
		for (PostDefend callback : callbacks) {
			callback.onTriggerResolve(victim, intent);
		}
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.EffectEnemyTarget#hits
	 * (net.wombatrpgs.saga.rpg.battle.Battle, net.wombatrpgs.saga.rpg.chara.Chara,
	 * net.wombatrpgs.saga.rpg.chara.Chara, int, float)
	 */
	@Override
	protected final boolean hits(Battle battle, Chara user, Chara victim, int power, float roll) {
		String tab = SConstants.TAB;
		String username = user.getName();
		String victimname = victim.getName();
		
		if (effect(OffenseFlag.ONLY_AFFECT_UNDEAD) && !victim.is(Flag.UNDEAD)) {
			// Enemy is exempt
			battle.println(tab + "Nothing happens.");
			return false;
			
		} else if (effect(OffenseFlag.ONLY_AFFECT_HUMANS) && victim.getRace() != Race.HUMAN) {
			// Enemy is exempt
			battle.println(tab + "Nothing happens.");
			return false;
			
		} else if (!combatHits(battle, user, victim, roll)) {
			// We missed... why?
			if (shielding(battle, victim) > 0) {
				List<EffectDefend> defenses = battle.getDefenses(victim);
				Collections.sort(defenses);
				EffectDefend blocker = defenses.get(0);
				CombatItem shield = blocker.getItem();
				shield.onBlockedWith(battle, victim);
			} else {
				battle.println(tab + username + " misses " + victimname + ".");
			}
			return false;
			
		} else {
			// We actually hit the jerk
			return true;
		}
	}
	
	/**
	 * Calculates the damage this attack would do against a hypothetical target.
	 * Does not actually deal damage. Is not affected by RNG.
	 * @param	battle			The battle this calculation is a part of
	 * @param	power			The output power of the effect (before def)
	 * @param	target			The target to check against
	 * @return					An appropriate amount of damage to deal, in HP
	 */
	protected abstract int calcDamage(Battle battle, int power, Chara target);
	
	/**
	 * Determines if this attack should hit a target via some physical combat
	 * formula. Does not actually deal damage. Is not affected by RNG. Called
	 * once per victim. No need to print a message, we'll figure out why.
	 * @param	battle			The battle context of this check
	 * @param	user			The character using the ability
	 * @param	target			The target to check against
	 * @param	roll			The "chance to miss" roll of the user
	 * @return					True if the attack should hit, false for miss
	 */
	protected abstract boolean combatHits(Battle battle, Chara user, Chara target, float roll);
	
	/**
	 * Checks if this attack is a physical attack (ie, it includes defense of
	 * the target somewhere in the calculation). Only important for mutations.
	 * @return					True if effect is physical, false otherwise
	 */
	protected abstract boolean isPhysical();
	
	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#onRoundStart
	 * (net.wombatrpgs.saga.rpg.battle.Intent)
	 */
	@Override
	public void onRoundStart(Intent intent) {
		super.onRoundStart(intent);
		for (EffectDefend rider : riderEffects) {
			List<Chara> targets = new ArrayList<Chara>();
			targets.add(intent.getActor());
			rider.setDefenders(intent.getBattle(), targets);
		}
	}
	
	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#onBlockedWith
	 * (net.wombatrpgs.saga.rpg.battle.Battle, net.wombatrpgs.saga.rpg.chara.Chara)
	 */
	@Override
	public void onBlockedWith(Battle battle, Chara victim) {
		riderEffects.get(0).onBlockedWith(battle, victim);
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.EffectEnemyTarget#onResolveComplete
	 * (Battle, Chara)
	 */
	@Override
	protected void onResolveComplete(Battle battle, Chara user) {
		super.onResolveComplete(battle, user);
		if (effect(OffenseFlag.KILLS_USER)) {
			user.damage(user.get(Stat.HP), false);
			battle.checkDeath(user, false);
		}
	}

	/**
	 * Checks if the effect flag is present.
	 * @param	flag			The flag to check
	 * @return					True if the flag is present, false otherwise
	 */
	protected boolean effect(OffenseFlag flag) {
		return hasFlag(mdo.sideEffects, flag);
	}
	
	/**
	 * Checks if the target resists this effect. Immune charas also resist.
	 * @param	target			The target to check
	 * @return					True if the target is resistant, else false
	 */
	protected boolean resists(Chara target) {
		return target.resists(mdo.damType); 
	}
	
	/**
	 * Checks if the target is immune to this effect.
	 * @param	target			The target to check
	 * @return					True if the target is immune
	 */
	protected boolean isImmune(Chara target) {
		return target.isImmuneTo(mdo.damType);
	}
	
	/**
	 * Checks if the target is weak to this effect.
	 * @param	target			The target to check
	 * @return					True if the target is weak, else false
	 */
	protected boolean weak(Chara target) {
		if (target.isWeakTo(mdo.damType)) {
			return true;
		}
		if (target.getFamily() != null) {
			for (String familyKey : mdo.slayerFamiles) {
				if (target.getFamily().getKey().equals(familyKey)) {
					return true;
				}
			}
		}
		return false;
	}
	
	/**
	 * Checks the shield value of a character.
	 * @param	battle			The battle context of this call
	 * @param	chara			The character to check
	 * @return					The shield value dodge boost for that character
	 */
	protected int shielding(Battle battle, Chara chara) {
		int shielding = 0;
		for (EffectDefend defense : battle.getDefenses(chara)) {
			shielding += defense.getShielding();
		}
		return shielding;
	}
	
	/**
	 * Combat formula potentially involving a stat as the power mult. Relies on
	 * the RNG.
	 * @param	user			The chara using the effect
	 * @param	powerStat		The stat to multiply with, or null for none
	 * @param	power			The base multiplier for damage
	 * @return					The power of the attack, in HP damage
	 */
	protected int statDamage(Chara user, Stat powerStat, int power) {
		int temp = power;
		int result = 0;
		if (powerStat != null) {
			temp *= Math.round((float) user.get(powerStat) / 4f);
			result = user.get(powerStat);
		}
		if (temp > 2) {
			result += (temp + MGlobal.rand.nextInt(temp / 2));
		}
		return result;
	}

}
