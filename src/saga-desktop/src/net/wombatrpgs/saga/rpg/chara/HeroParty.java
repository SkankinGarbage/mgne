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
import net.wombatrpgs.sagaschema.rpg.stats.Stat;

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
	
	/**
	 * Creates a hero party from serialized memory.
	 * @param	memory			The memory to load from
	 */
	public HeroParty(PartyMemory memory) {
		this.gp = memory.gp;
		this.location = memory.location;
		this.inventory = new PartyInventory(this, memory.inventory);
		this.collection = memory.collectables;
		
		heroes = new ArrayList<Chara>();
		for (int i = 0; i < memory.charas.length; i += 1) {
			groups.add(new ArrayList<Chara>());
		}
		for (int i = 0; i < memory.charas.length; i += 1) {
			CharaMemory charaMemory = memory.charas[i];
			int groupIndex = memory.charaOrderIndices[i];
			Chara chara = null;
			if (groupIndex == -1) {
				// shit this is busted save from v1.00
				// this is a 5th party member who should've been removed
				// replace them with the next member up
				groupIndex = 4;
				
				String replacementKey = null;
				if (charaMemory.name.equals("BORG")) {
					replacementKey = "chara_david";
				} else if (charaMemory.name.equals("David")) {
					if (charaMemory.stats.stat(Stat.MHP) < 100) {
						replacementKey = "chara_lara";
					} else {
						replacementKey = "chara_borg2";
					}
				} else if (charaMemory.name.equals("Lara")) {
					replacementKey = "chara_zkauba";
				} else if (charaMemory.name.equals("Zkauba")) {
					replacementKey = "chara_david2";
				} else if (charaMemory.name.equals("BORG Mk2")) {
					replacementKey = "chara_jonas";
				} else if (charaMemory.name.equals("Jonas")) {
					replacementKey = "chara_janine";
				} else {
					MGlobal.reporter.err("Corrupted save loading couldn't load replacement for " + charaMemory.name);
					return;
				}
				chara = new Chara(replacementKey);
				
			} else {
				chara = new Chara(charaMemory);
			}
			assets.add(chara);
			heroes.add(chara);
			groups.get(groupIndex).add(chara);
		}
		rebuildMembers();
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
	
	/** @return The party's collectable set */
	public CollectableSet getCollection() { return collection; }
	
	/**	@param slot The slot of the hero to fetch (0 for protagonist)
	 *	@return The nth hero of the story */
	public Chara getStoryHero(int slot) { return heroes.get(slot); }

	/**
	 * @see net.wombatrpgs.saga.rpg.chara.Party#isCarryingItemType(java.lang.String)
	 */
	@Override
	public boolean isCarryingItemType(String itemKey) {
		return super.isCarryingItemType(itemKey) || inventory.containsItemType(itemKey);
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.chara.Party#swap(int, int)
	 */
	@Override
	public void swap(int index1, int index2) {
		super.swap(index1, index2);
		setLeaderAppearance();
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.chara.Party#innlikeHeal()
	 */
	@Override
	public void innlikeHeal() {
		super.innlikeHeal();
		setLeaderAppearance();
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
		boolean removed = heroes.remove(hero);
		if (!removed) {
			System.out.println();
		}
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
	
	/**
	 * Dead heroes should be put in the back.
	 */
	public void reorderDeadHeroes() {
		// somewhat intentionally disabled -- this is annoying
		// originally thought it was buggy but apparently it's bugged in the "noop"
		// ...rather than the "save corruption" one
//		List<Chara> oldHeroes = new ArrayList<Chara>();
//		for (Chara hero : oldHeroes) {
//			if (hero.isDead()) {
//				insert(hero, 4); // last player-controlled slot in party
//			}
//		}
		setLeaderAppearance();
	}
	
	/**
	 * Sets the avatar's appearance to match the party leader.
	 */
	public void setLeaderAppearance() {
		MGlobal.getHero().setAppearance(findLeader().getAppearance());
	}
	
	/**
	 * Puts a character at specific location in the party, moving everyone else
	 * back by an slot
	 * @param	hero			The hero to insert
	 * @param	index			The index to insert them at, 0 is front
	 */
	public void insert(Chara hero, int index) {
		MGlobal.reporter.err("insert disabled, probably buggy");
//		if (contains(hero)) {
//			removeHero(hero);
//		}
//		List<Chara> oldHeroes = new ArrayList<Chara>();
//		oldHeroes.addAll(heroes);
//		groups.clear();
//		members.clear();
//		heroes.clear();
//		int existingIndex = 0;
//		for (int i = 0; i < groups.size()+1; i += 1) {
//			if (index == i) {
//				addHero(hero);
//			} else {
//				addHero(oldHeroes.get(existingIndex));
//				existingIndex += 1;
//			}
//		}
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.chara.Party#instantiateChara(java.lang.String)
	 */
	@Override
	protected Chara instantiateChara(String mdoKey) {
		return new Chara(mdoKey, false);
	}

}
