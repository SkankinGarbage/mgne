/**
 *  EffectDefend.java
 *  Created on Apr 25, 2014 7:39:41 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.warheads;

import java.util.List;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.maps.MapThing;
import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.rpg.battle.Battle;
import net.wombatrpgs.saga.rpg.battle.Intent;
import net.wombatrpgs.saga.rpg.chara.Chara;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.saga.rpg.stats.SagaStats;
import net.wombatrpgs.saga.screen.TargetSelectable;
import net.wombatrpgs.sagaschema.rpg.abil.CombatItemMDO;
import net.wombatrpgs.sagaschema.rpg.abil.data.DamageType;
import net.wombatrpgs.sagaschema.rpg.abil.data.DefenseFlag;
import net.wombatrpgs.sagaschema.rpg.warheads.EffectDefendMDO;

/**
 * Increases physical attack evasion value and potentially grants other resists.
 */
public class EffectDefend extends EffectAllyTarget implements Comparable<EffectDefend> {

	protected EffectDefendMDO mdo;
	protected CombatItem counterItem;
	protected String defendName;
	protected boolean silent;
	
	/**
	 * Inherited constructor.
	 * @param	mdo				The data to create from
	 * @param	item			The data to create for
	 */
	public EffectDefend(EffectDefendMDO mdo, CombatItem item) {
		super(mdo, item);
		this.mdo = mdo;
		
		if (MapThing.mdoHasProperty(mdo.defendName)) {
			defendName = MGlobal.charConverter.convert(mdo.defendName);
		} else if (item != null) {
			defendName = item.getName();
		} else {
			silent = true;
		}
		
		if (mdo.warhead != null) {
			AbilEffect counter = AbilEffectFactory.create(mdo.warhead.key, null);
			assets.add(counter);
			
			CombatItemMDO newMDO = new CombatItemMDO();
			newMDO.abilityName = defendName;
			newMDO.uses = 0;
			counterItem = new CombatItem(newMDO, counter);
			
			counter.setItem(counterItem);
		}
	}
	
	/**
	 * Creates a new silent rider defense effect.
	 * @param	mdo				The data to create from
	 */
	public EffectDefend(EffectDefendMDO mdo) {
		this(mdo, null);
	}

	/** @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#isMapUsable() */
	@Override public boolean isMapUsable() { return false; }
	
	/** @return The dodge-increase shield rating of this defense */
	public int getShielding() { return mdo.shielding; }

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#onMapUse
	 * (net.wombatrpgs.saga.screen.TargetSelectable)
	 */
	@Override
	public void onMapUse(TargetSelectable caller) {
		MGlobal.reporter.err("Unusable ability");
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#setItem(net.wombatrpgs.saga.rpg.items.CombatItem)
	 */
	@Override
	public void setItem(CombatItem item) {
		super.setItem(item);
		defendName = item.getName();
	}

	/**
	 * @see java.lang.Comparable#compareTo(java.lang.Object)
	 */
	@Override
	public int compareTo(EffectDefend other) {
		return other.getShielding() - getShielding();
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#resolve
	 * (net.wombatrpgs.saga.rpg.battle.Intent)
	 */
	@Override
	public void resolve(Intent intent) {
		if (!silent) {
			Battle battle = intent.getBattle();
			Chara user = intent.getActor();
			String username = user.getName();
			CombatItem item = intent.getItem();
			String itemname = item.getName();
			Chara victim = intent.getTargets().get(0);
			String defendname = "'";
			switch (mdo.projector) {
			case ALLY_PARTY:
				defendname = "all ";
				break;
			case PLAYER_PARTY_ENEMY_GROUP:
				defendname = "group ";
				break;
			case SINGLE_ALLY:
				defendname = victim.getName() + " ";
				break;
			case USER:
				defendname = "";
				break;
			}
			battle.println(username + " defends " + defendname + "by " + itemname + ".");
		}
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#onRoundStart
	 * (net.wombatrpgs.saga.rpg.battle.Intent)
	 */
	@Override
	public void onRoundStart(Intent intent) {
		super.onRoundStart(intent);
		setDefenders(intent.getBattle(), intent.getTargets());
	}
	
	/**
	 * Covers the targets with this defense's effect.
	 * @param	battle			The context to apply the effect in
	 * @param	targets			The targets to protect
	 */
	public void setDefenders(Battle battle, List<Chara> targets) {
		for (Chara target : targets) {
			battle.applyDefendBoost(target, new SagaStats(mdo.stats));
			battle.applyDefense(target, this);
		}
	}
	
	/**
	 * Called when a character using this defense is about to take damage.
	 * Respond if appropriate by countering.
	 * @param	victim			The character about to be hit
	 * @param	damType			The type of damage about to be taken
	 * @param	attackIntent	The intent of the attacker
	 * @return					True if the attack is countered and doesn't hit
	 */
	public DefendResult onAttack(Chara victim, Intent attackIntent, DamageType damType) {
		final Battle battle = attackIntent.getBattle();
		final String victimname = victim.getName();
		final String tab = SConstants.TAB;
		if (hasFlag(mdo.triggerTypes, damType)) {
			final boolean blocked = hasFlag(mdo.effects, DefenseFlag.BLOCKS_TRIGGERING_DAMAGE);
			final Intent counterIntent = new Intent(victim, battle);
			counterIntent.addTargets(attackIntent.getActor());
			counterIntent.setRecursive(true);
			if (hasFlag(mdo.effects, DefenseFlag.REFLECTS_TRIGGERING_ATTACK)) {
				CombatItem reflectAbil = attackIntent.getItem();
				counterIntent.setItem(reflectAbil);
				reflectAbil.incrementUses();
			} else if (counterItem != null) {
				counterIntent.setItem(counterItem);
			}
			return new DefendResult(blocked, new PostDefend() {
				@Override public void onTriggerResolve(Chara victim, Intent attackIntent) {
					if (victim.isAlive()) {
						if (blocked) {
							battle.print(tab + "but");
						}
						battle.println(victimname + " counters by " + defendName + ".");
						if (counterIntent.getItem() != null) {
							counterIntent.resolve();
						}
					}
				}
			});
		} else {
			return new DefendResult(false, null);
		}
	}
	
	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#onBlockedWith(Battle, Chara)
	 */
	@Override
	public void onBlockedWith(Battle battle, Chara victim) {
		final String tab = SConstants.TAB;
		final String victimname = victim.getName();
		battle.println(tab + victimname + " deflects by " + defendName + ".");
	}

	/**
	 * Struct to return to EffectCombat.
	 */
	public static class DefendResult {
		
		public final boolean countered;
		public final PostDefend callback;
		
		/**
		 * Creates a new DefendResult with all values.
		 * @param	countered		True if the attack was actually countered
		 * @param	callback		The callback for trigger, null is fine
		 */
		public DefendResult(boolean countered, PostDefend callback) {
			this.countered = countered;
			this.callback = callback;
		}
	}
	
	/**
	 * Callback for defense after resoluton.
	 */
	public static abstract class PostDefend {
		
		/**
		 * Called when the attack that triggered the counter is resolved.
		 * @param	victim			The character that was attacked
		 * @param	attackIntent	The intent of the attacker
		 */
		public abstract void onTriggerResolve(Chara victim, Intent attackIntent);
		
	}

}
