/**
 *  SGlobal.java
 *  Created on Apr 4, 2014 6:35:33 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.core;

import java.util.ArrayList;
import java.util.List;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.Queueable;
import net.wombatrpgs.saga.SagaSettings;
import net.wombatrpgs.saga.graphics.SGraphics;
import net.wombatrpgs.saga.maps.TeleportMemory;
import net.wombatrpgs.saga.rpg.chara.HeroParty;
import net.wombatrpgs.sagaschema.rpg.chara.PartyMDO;

/**
 * The counterpart to MGlobal. Holds Saga-related global information.
 */
public class SGlobal {
	
	/** RPG information */
	public static HeroParty heroes;
	public static List<TeleportMemory> teleports;
	
	/** Settings and default keys */
	public static SagaSettings settings;
	
	/** Saga-specific graphics */
	public static SGraphics graphics;
	
	/** Miscellaneous globals */
	public static int saveSlot;
	public static String battleBGMKey;
	
	/**
	 * Load the game settings. Call immediately after data is initialized.
	 */
	public static void preLoad() {
		settings = new SagaSettings();
		teleports = new ArrayList<TeleportMemory>();
	}
	
	/**
	 * Sets up all the global variables. Called once when game is created.
	 */
	public static void globalInit() {
		
		List<Queueable> toLoad;
		toLoad = new ArrayList<Queueable>();
		
		// then everything else
		graphics = (SGraphics) MGlobal.graphics;
		String partyKey = MGlobal.args.get("party");
		if (partyKey == null) {
			heroes = new HeroParty();
		} else {
			heroes = new HeroParty(MGlobal.data.getEntryFor(partyKey, PartyMDO.class));
		}
		toLoad.add(heroes);
		MGlobal.assets.loadAssets(toLoad, "SGlobal");
		
		// debug save-y stuff
		String savefile = MGlobal.args.get("savefile");
		if (savefile != null) {
			MGlobal.memory.loadAndSetScreen(savefile);
		}
		saveSlot = -1;
	}

}
