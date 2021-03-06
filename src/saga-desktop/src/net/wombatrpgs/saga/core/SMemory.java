/**
 *  SMemory.java
 *  Created on Apr 4, 2014 8:00:28 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.core;

import java.util.ArrayList;
import java.util.List;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.Memory;
import net.wombatrpgs.saga.maps.TeleportMemory;
import net.wombatrpgs.saga.rpg.chara.HeroParty;
import net.wombatrpgs.saga.rpg.chara.PartyMemory;

/**
 * Saga memory for storing specialized saga data in mgne.
 */
public class SMemory extends Memory {
	
	// fields to store/unload
	public PartyMemory partyMemory;
	public String battleBGMKey;
	public List<TeleportMemory> teleportLocations;
	public int saveSlot;
	
	/**
	 * Creates a new, empty memory.
	 */
	public SMemory() {
		teleportLocations = new ArrayList<TeleportMemory>();
	}
	
	/**
	 * @see net.wombatrpgs.mgne.core.Memory#storeFields()
	 */
	@Override
	protected void storeFields() {
		super.storeFields();
		this.partyMemory = new PartyMemory(SGlobal.heroes);
		this.saveSlot = SGlobal.saveSlot;
		this.battleBGMKey = SGlobal.battleBGMKey;
		this.teleportLocations = SGlobal.teleports;
	}

	/**
	 * @see net.wombatrpgs.mgne.core.Memory#unloadFields()
	 */
	@Override
	protected void unloadFields() {
		SGlobal.heroes = new HeroParty(this.partyMemory);
		SGlobal.saveSlot = this.saveSlot;
		SGlobal.battleBGMKey = this.battleBGMKey;
		SGlobal.teleports = this.teleportLocations;
		super.unloadFields();
	}

	/**
	 * @see net.wombatrpgs.mgne.core.Memory#loadAssets()
	 */
	@Override
	protected void loadAssets() {
		MGlobal.assets.loadAsset(SGlobal.heroes, "loaded heroes");
		super.loadAssets();
	}

}
