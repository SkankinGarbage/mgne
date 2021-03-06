/**
 *  CombatItem.java
 *  Created on Apr 12, 2014 2:49:18 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.items;

import java.util.EnumSet;

import net.wombatrpgs.mgne.core.AssetQueuer;
import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.maps.MapThing;
import net.wombatrpgs.saga.rpg.battle.Battle;
import net.wombatrpgs.saga.rpg.battle.Intent;
import net.wombatrpgs.saga.rpg.battle.Intent.IntentListener;
import net.wombatrpgs.saga.rpg.chara.Chara;
import net.wombatrpgs.saga.rpg.mutant.MutantEvent;
import net.wombatrpgs.saga.rpg.stats.SagaStats;
import net.wombatrpgs.saga.rpg.warheads.AbilEffect;
import net.wombatrpgs.saga.rpg.warheads.AbilEffectFactory;
import net.wombatrpgs.saga.screen.TargetSelectable;
import net.wombatrpgs.sagaschema.graphics.banim.data.BattleAnimMDO;
import net.wombatrpgs.sagaschema.rpg.abil.CombatItemMDO;
import net.wombatrpgs.sagaschema.rpg.abil.data.AbilityType;
import net.wombatrpgs.sagaschema.rpg.abil.data.EquipmentFlag;

/**
 * Represents the combat item MDO. This could be an item or ability, but either
 * way, it can be equipped to a character and potentially have a use in combat.
 */
public class CombatItem extends AssetQueuer {
	
	protected CombatItemMDO mdo;
	
	protected Inventory container;
	protected AbilEffect effect;
	protected String name;
	protected EnumSet<EquipmentFlag> equipFlags;
	protected SagaStats stats, robostats;
	protected int uses, usesWhenAdded;
	
	/**
	 * Creates a new combat item from data.
	 * @param	mdo				The data to create from
	 */
	public CombatItem(CombatItemMDO mdo) {
		this.mdo = mdo;
		if (mdo.uses == null) {
			MGlobal.reporter.err("MDO needs uses: " + mdo);
		}
		uses = mdo.uses;
		name = MGlobal.charConverter.convert(mdo.abilityName);
		
		stats = new SagaStats();
		robostats = new SagaStats(mdo.robostats);
		
		// this case should only happen for defense combat items
		if (mdo.warhead != null) {
			effect = AbilEffectFactory.create(mdo.warhead.key, this);
			assets.add(effect);
		}
		
		if (mdo.equip != null && mdo.equip.length > 0) {
			equipFlags = EnumSet.of(mdo.equip[0], mdo.equip);
		} else {
			equipFlags = EnumSet.noneOf(EquipmentFlag.class);
		}
	}
	
	/**
	 * Creates a new combat item from serialized item data.
	 * @param	memory			The saved data to restore from
	 */
	public CombatItem(ItemMemory memory) {
		this(memory.key);
		this.uses = memory.uses;
		this.usesWhenAdded = memory.usesWhenAdded;
	}
	
	/**
	 * Creates a combat item with a fixed effect. Does not queue the effect.
	 * @param	mdo				The data to create from
	 * @param	effect			The effect to create with, must not be null
	 */
	public CombatItem(CombatItemMDO mdo, AbilEffect effect) {
		this(mdo);
		this.effect = effect;
		if (effect == null) {
			MGlobal.reporter.err("Null effect for mdo: " + mdo);
		} else {
			effect.setItem(this);
		}
	}
	
	/**
	 * Creates a new combat item from a key to data.
	 * @param	key				The key to the data to create from
	 */
	public CombatItem(String key) {
		this(MGlobal.data.getEntryFor(key, CombatItemMDO.class));
	}
	
	/** @return The ability name of this item */
	public String getName() { return name; }
	
	/** @return True if this item has unlimited uses */
	public boolean isUnlimited() { return mdo.uses == 0; }
	
	/** @return The number of uses remaining on this item */
	public int getUses() { return uses; }
	
	/** @return True if this item can be sold to a shop, false otherwise */
	public boolean isSellable() { return mdo.cost > 0; }
	
	/** @return The global stats this item imbues when equipped */
	public SagaStats getStatset() { return stats; }
	
	/** @return The robot-specific stat boosts this item imbues when equipped */
	public SagaStats getRobostats() { return robostats; }
	
	/** @return The MDO key of this item, used to compare for equality */
	public String getKey() { return mdo.key; }
	
	/** @return The type (item or ability) of this combat item */
	public AbilityType getType() { return mdo.type; }
	
	/** @param inventory The inventory that contains this item */
	public void setContainer(Inventory inventory) { this.container = inventory; }
	
	/** @return The number of uses this item had when equipped */
	public int getAddedUses() { return usesWhenAdded; }
	
	/** @return The item description (for noob mode) */
	public String getDescription() { return mdo.itemDescription == null ? getName() : mdo.itemDescription; }
	
	/** @return True if this item can target the dead */
	public boolean canTargetDead() { return effect.canTargetDead(); }

	/**
	 * @see java.lang.Object#toString()
	 */
	@Override
	public String toString() {
		return getName();
	}
	
	/**
	 * Called by the inventory when this item is added to it. Call for null to
	 * remove from any inventories.
	 * @param	inventory		The inventory added to, or null for removed
	 */
	public void onAddedTo(Inventory inventory) {
		this.container = inventory;
		usesWhenAdded = uses;
	}
	
	/**
	 * Called when this item is used to block. Only relevant for shields really.
	 * @param	battle			The battle where the block occurred
	 * @param	victim			The victim being attacked
	 */
	public void onBlockedWith(Battle battle, Chara victim) {
		effect.onBlockedWith(battle, victim);
	}

	/**
	 * Called when this item is used from the map menu.
	 * @param	caller			The interface used to get other targets for this
	 */
	public void onMapUse(TargetSelectable caller) {
		ensureEffect();
		effect.onMapUse(caller);
	}
	
	/**
	 * Check if this item can be used on the world map.
	 * @return					True if this item can be used on the map
	 */
	public boolean isMapUsable() {
		ensureEffect();
		return effect.isMapUsable();
	}
	
	/**
	 * Check if this item can be used in battle (a true combat item!)
	 * @return					True if this item can be used in battle
	 */
	public boolean isBattleUsable() {
		ensureEffect();
		return effect.isBattleUsable();
	}
	
	/**
	 * Construct an intent from player input and warhead. This is called by the
	 * battle when this item is selected for use, and later passed back when
	 * the item is used in battle playback. The intent might already have some
	 * preselected options and the player is editing it, or it might be raw.
	 * @param	intent			The currently recorded intent state, never null
	 * @param	listener		The listener to give intent to when finished
	 */
	public void modifyIntent(Intent intent, IntentListener listener) {
		effect.modifyIntent(intent, listener);
	}
	
	/**
	 * Construct an intent for an enemy. This is the AI method, anything that
	 * affects how the AI uses this specific item should be changed here. This
	 * differs from the player method because no callback is needed, AI decides
	 * immediately. Just the target needs to be selected usually.
	 * @param	intent			The intent to modify
	 */
	public void modifyEnemyIntent(Intent intent) {
		intent.setItem(this);
		effect.modifyEnemyIntent(intent);
	}
	
	/**
	 * Picks random targets for this item as if the owner were confused and
	 * attacking charas at random from both sides... hm, when could this be
	 * useful? Rather than return target list, add them to the intent. If no
	 * targets are available, do nothing.
	 * @param	intent			The intent to modify
	 */
	public void assignRandomTargets(Intent intent) {
		intent.setItem(this);
		intent.clearTargets();
		effect.assignRandomTargets(intent);
	}
	
	/**
	 * Called when a round begins in which this item is involved. This is used
	 * to do things that should occur at the round start, before the item is
	 * actually resolved. Most of the times it can do nothing.
	 * @param	intent			The intent that will be resolved later this turn
	 */
	public void onRoundStart(Intent intent) {
		effect.onRoundStart(intent);
	}
	
	/**
	 * Resolves a previous intent given in battle. Preconditions such as status
	 * of user are handled elsewhere. Just do what this item does.
	 * @param	intent			The intent to resolve
	 */
	public void resolve(Intent intent) {
		effect.resolve(intent);
		if (intent.getActor().knowsAsAbil(this)) {
			intent.getActor().recordEvent(MutantEvent.USED_ABILITY);
		}
		deductUse();
	}
	
	/**
	 * Animates this item being used as part of an intent. Meant to be called
	 * by the intent during its resolution.
	 * @param	intent			The intent to animate
	 */
	public void animate(Intent intent) {
		if (MapThing.mdoHasProperty(mdo.anim)) {
			BattleAnimMDO animMDO = MGlobal.data.getEntryFor(mdo.anim, BattleAnimMDO.class);
			intent.getBattle().animate(intent.getActor(), animMDO, intent.getTargets());
		}
	}
	
	/**
	 * Halves the uses on this item, like if a robot equipped it.
	 */
	public void halveUses() {
		uses = (int) Math.floor((float) uses / 2f);
		checkDiscard();
	}
	
	/**
	 * Gives the item an extra use... like if someone reflected the attack. This
	 * is kind of janky in retrospect.
	 */
	public void incrementUses() {
		uses += 1;
	}
	
	/**
	 * Restores this item to the condition it was at when it was equipped. Note
	 * that this is not an ARCANE-style restore, it's meant to be used for robot
	 * equips and abilities mostly.
	 */
	public void restoreUses() {
		uses = usesWhenAdded;
	}
	
	/**
	 * The actual arcane-style item restore. Restores uses back to the item's
	 * maximum amount. Meant to be used for items and not robot equipment or
	 * abilities.
	 */
	public void restoreUsesArcane() {
		uses = mdo.uses;
	}
	
	/**
	 * Called internally when the item is used in battle, or by the effect when
	 * applied on the world map. Simulates wear on the item and removes it the
	 * item reaches zero uses.
	 */
	public void deductUse() {
		uses -= 1;
		checkDiscard();
	}
	
	/**
	 * Calculates the price to buy the item at the current number of uses. Uses
	 * the standard uses/max * cost formula.
	 * @param	sellMode		True if the item is being sold back at 1/2 cost
	 * @return					The cost to buy the item, in GP
	 */
	public int getCost(boolean sellMode) {
		int cost = mdo.cost;
		if (mdo.uses > 0) {
			cost *= uses;
			cost /= mdo.uses;
		}
		if (sellMode) {
			cost /= 2;
		}
		return cost;
	}
	
	/**
	 * Calculates the price to buy the item at the current number of uses by
	 * calling the relevant inventory's price valuation. Will blow up if the
	 * item isn't owned by anything.
	 * @return					The cost to buy the item, in GP
	 */
	public int getCost() {
		if (container == null) {
			MGlobal.reporter.warn("No container for item: " + this);
			return -1;
		} else {
			return container.valueOf(this);
		}
	}
	
	/**
	 * Checks if this item shares an equipemnt flag with another item, which
	 * would indicate they are both helmets etc.
	 * @param	other			The item to check against
	 * @return					True if the items share a flag, false otherwise
	 */
	public boolean sharesFlagWith(CombatItem other) {
		for (EquipmentFlag flag : equipFlags) {
			if (other.equipFlags.contains(flag)) {
				return true;
			}
		}
		return false;
	}
	
	/**
	 * Checks if this item should be discarded, and if so, discards it.
	 */
	public void checkDiscard() {
		if (container == null) return;
		if (uses > 0) return;
		if (mdo.uses == 0) return;
		if (mdo.type == AbilityType.ABILITY) return;
		
		container.checkDiscard(this);
	}
	
	/**
	 * Makes sure the item effect is non-null.
	 */
	protected void ensureEffect() {
		if (effect == null) {
			MGlobal.reporter.err("Null effect on item: " + mdo.abilityName);
		}
	}

}
