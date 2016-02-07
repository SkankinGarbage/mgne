/**
 *  Intent.java
 *  Created on Apr 23, 2014 2:56:13 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.battle;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.saga.rpg.chara.Chara;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.sagaschema.rpg.stats.Stat;

/**
 * Class to keep track of what a player wants to do on a character's turn. Does
 * not have any verification, just passes straight back and forth from a combat
 * item. This is so that the same intent class can be used across all combat
 * items. When the player selects a combat item, that item is responsible for
 * producing an intent, then parsing the intent later on when it's that
 * character's turn in battle.
 * 
 * Note that the target list is a list of *possible* targets, not necessarily
 * everything that will get hit by the attack. A single-target attack targets
 * a group of monsters, so even if the front monster in that group dies, the
 * attack will hit someone.
 */
public class Intent implements Comparable<Intent> {
	
	protected Chara actor;
	protected CombatItem item;
	protected List<Chara> targets;
	protected Battle battle;
	protected int priority;
	protected boolean recursive;
	
	/**
	 * Creates a new intent for the designated actor.
	 * @param	actor			The chara that will be acting
	 * @param	battle			The battle this intent is a part of
	 */
	public Intent(Chara actor, Battle battle) {
		this.actor = actor;
		this.battle = battle;
		targets = new ArrayList<Chara>();
		priority = (int) actor.get(Stat.AGI);
		if (priority > 0) {
			priority += MGlobal.rand.nextInt(priority);
		}
	}
	
	/** @return All targeted characters */
	public List<Chara> getTargets() { return targets; }
	
	/** @param targets More targets of this combat item */
	public void addTargets(Chara... targets) { Collections.addAll(this.targets, targets); }
	
	/** @param targets More targets of this combat item */
	public void addTargets(List<Chara> targets) { this.targets.addAll(targets); }
	
	/** @return The battle this intent is a part of */
	public Battle getBattle() { return battle; }
	
	/** @return The chara this intent is for */
	public Chara getActor() { return actor; }
	
	/** @return The item this chara is using */
	public CombatItem getItem() { return item; }
	
	/** @param item The combat item the player selected for this intent */
	public void setItem(CombatItem item) { this.item = item; }
	
	/** Clears the targets list */
	public void clearTargets() { targets.clear(); }
	
	/** @return True if recrusive print mode is enabled */
	public boolean isRecursive() { return recursive; }
	
	/** @param recursive True to set recursive print mode for the intent */
	public void setRecursive(boolean recursive) { this.recursive = recursive; }
	
	/**
	 * @see java.lang.Comparable#compareTo(java.lang.Object)
	 */
	@Override
	public int compareTo(Intent other) {
		return other.priority - priority;
	}

	/**
	 * @see java.lang.Object#toString()
	 */
	@Override
	public String toString() {
		String actorname = actor.getName();
		if (item == null) {
			return actorname + " is undecided";
		} else {
			String itemname = item.getName();
			if (targets.size() > 0) {
				String targetname = targets.get(0).getName();
				return actor.getName() + " attacks " + targetname + "(s) by " +
						itemname;
			} else {
				return actor.getName() + " uses " + itemname;
			}
		}
	}

	/**
	 * Works out which group the user probably selected based on the targets.
	 * Returns 0 if no group selected, or -1 if the game is broken.
	 * @return					The index of the selected group, or 0 if none
	 */
	public int inferEnemy() {
		if (targets.size() == 0) return 0;
		return battle.getEnemy().index(targets.get(0));
	}
	
	/**
	 * Works out which allied group the user probably selected based on the
	 * targets. Returns 0 if no group selected, or -1 if the game is broken.
	 * @return					The index of the selected ally, or 0 if none
	 */
	public int inferAlly() {
		if (targets.size() == 0) return 0;
		return battle.getPlayer().index(targets.get(0));
	}
	
	/**
	 * Creates a new listener that can be used to interface with the battle's
	 * selection methods.
	 * @param	listener		The listener from the intent construction
	 * @return					A new listener to send to the battle
	 */
	public TargetListener genDefaultListener(final IntentListener listener) {
		final Intent intent = this;
		return new TargetListener() {
			@Override public void onTargetSelection(List<Chara> targets) {
				if (targets == null) {
					listener.onIntent(null);
				} else {
					intent.addTargets(targets);
					listener.onIntent(intent);
				}
			}
		};
	}
	
	/**
	 * Called when the round of which this intent is a process is begun. This
	 * hands off to the effect.
	 */
	public void onRoundStart() {
		if (item != null) {
			item.onRoundStart(this);
		}
	}
	
	/**
	 * Called when this intent is up to be interpreted. To keep the battle a bit
	 * more sane, all of the resolution code is done in here. This includes
	 * caster status effects and checking for dead targets.
	 */
	public void resolve() {
		
		// dead men tell no tales
		if (actor.isDead()) {
			return;
		}
		
		// sleeping ones shouldn't act
		if (!actor.canAct(battle, false, false)) {
			return;
		}
		
		// confused guys are confused
		if (actor.isConfused(battle, false, false)) {
			setItem(actor.getRandomCombatItem());
			if (item != null) {
				item.assignRandomTargets(this);
			}
		}
		
		// don't attack corpses, probably
		if (item != null && !item.canTargetDead()) {
			List<Chara> deads = new ArrayList<Chara>();
			for (Chara target : targets) {
				if (target.isDead()) {
					deads.add(target);
				}
			}
			for (Chara dead : deads) {
				targets.remove(dead);
			}
		}
		
		// don't attack empty space
		if (item == null || targets.isEmpty()) {
			battle.println(actor.getName() + " does nothing.");
			return;
		}
		
		// okay we have something to splatter!!
		item.resolve(this);
	}
	
	/**
	 * Waits for an intent to be constructed.
	 */
	public static interface IntentListener {
		
		/**
		 * Called when the player is done indicating their intent.
		 * @param	intent			The constructed intent, or null if canceled
		 */
		public void onIntent(Intent intent);
		
	}
	
	/**
	 * Listener for when targets are selected. Used for both groups and singles.
	 */
	public static interface TargetListener {
		
		/**
		 * Called when target(s) selected. Could be called with null if the
		 * player decided to cancel instead. Should probably never be empty?
		 * @param	targets			The target(s) the player selected, or null
		 */
		public void onTargetSelection(List<Chara> targets);
		
	}

}
