/**
 *  HeroParty.java
 *  Created on Apr 4, 2014 7:49:30 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.chara;

import java.util.ArrayList;
import java.util.List;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.items.CollectableSet;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.saga.rpg.items.PartyInventory;
import net.wombatrpgs.sagaschema.rpg.chara.PartyMDO;

/**
 * The player party. Contains four or five dauntless heroes.
 */
public class HeroParty extends Party {
	
	protected int gp;
	protected String location;
	protected PartyInventory inventory;
	protected CollectableSet collection;
	protected List<Chara> heroes;
	
	/**
	 * The hero party. Really should only be called once at the beginning of the
	 * game. Oh and most likely it'll be empty except for testing purposes.
	 * @param	mdo				The data to start with
	 */
	public HeroParty(PartyMDO mdo) {
		super(mdo);
		inventory = new PartyInventory(this);
		collection = new CollectableSet();
		heroes = new ArrayList<Chara>();
		for (Chara hero : getAll()) {
			heroes.add(hero);
		}
	}
	
	/**
	 * Creates the hero party by looking the default up in the database.
	 */
	public HeroParty() {
		this(MGlobal.data.getEntryFor(SGlobal.settings.getStartingPartyKey(),
				PartyMDO.class));
	}
	
	/** @return The name of the location of the party in the world */
	public String getLocation() { return location; }
	
	/** @param name The new name of the location of the party in the world */
	public void setLocation(String name) { this.location = name; }
	
	/** @return The amount of gold carried by the party */
	public int getGP() { return gp; }
	
	/** @param gp The amount of gold to give to the party */
	public void addGP(int gp) { this.gp += gp; }
	
	/** @param gp The amount of gold to take from the party, assumes enough */
	public void removeGP(int gp) { this.gp -= gp; }
	
	/** @return The inventory of the party */
	public PartyInventory getInventory() { return inventory; }
	
	/** @retrun The front member of the party */
	public Chara getFront() { return getFront(0); }
	
	/** @return The party's collectable set */
	public CollectableSet getCollection() { return collection; }
	
	/** @return The nth hero of the story (0 for protagonist) */
	public Chara getStoryHero(int slot) { return heroes.get(slot); }

	/**
	 * @see net.wombatrpgs.saga.rpg.chara.Party#isCarryingItemType(java.lang.String)
	 */
	@Override
	public boolean isCarryingItemType(String itemKey) {
		return super.isCarryingItemType(itemKey) || inventory.containsItemType(itemKey);
	}

	/**
	 * Adds a character to the party.
	 * @param	hero			The character to add
	 */
	public void addHero(Chara hero) {
		List<Chara> group = new ArrayList<Chara>();
		group.add(hero);
		groups.add(group);
		members.add(hero);
		assets.add(hero);
		heroes.add(hero);
	}
	
	/**
	 * Removes a hero from the party, usually the 5th temporary member.
	 * @param	hero			The character to remove
	 */
	public void removeHero(Chara hero) {
		assets.remove(hero);
		members.remove(hero);
		List<Chara> toRemove = null;
		for (List<Chara> group : groups) {
			if (group.contains(hero)) {
				toRemove = group;
				break;
			}
		}
		if (toRemove == null) {
			MGlobal.reporter.warn("No group found when removing hero " + hero);
		} else {
			groups.remove(toRemove);
		}
	}
	
	/**
	 * Finds the first non-dead party member. He's the leader. Don't call this
	 * when the party is all dead, please.
	 * @return					The first non-dead member, or null if all dead
	 */
	public Chara findLeader() {
		for (int i = 0; i < groupCount(); i += 1) {
			if (getFront(i).isAlive()) {
				return getFront(i);
			}
		}
		return null;
	}
	
	/**
	 * Adds a combat item to the party's inventory.
	 * @param	item			The item to add
	 * @return					True if it was added, false if we were full
	 */
	public boolean addItem(CombatItem item) {
		return getInventory().add(item);
	}

}
