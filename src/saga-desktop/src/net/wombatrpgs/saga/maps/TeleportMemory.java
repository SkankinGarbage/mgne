/**
 *  TeleportMemory.java
 *  Created on Jul 12, 2015 3:26:20 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.maps;

/**
 * POJO for someplace the hero can teleport to.
 */
public class TeleportMemory {
	
	public String mapKey;
	public String displayName;
	public int tileX, tileY;
	
	/**
	 * Serialization constructor.
	 */
	public TeleportMemory() {
		
	}
	
	/**
	 * Creates a new teleport memory. Does not insert it into the save data.
	 * @param	mapKey			The key that the target map is loaded by
	 * @param	displayName		The display name of this teleport in selection
	 * @param	tileX			The x-coord to teleport to, in tiles
	 * @param	tileY			The y-coord to teleport to, in tiles
	 */
	public TeleportMemory(String mapKey, String displayName, int tileX, int tileY) {
		this.mapKey = mapKey;
		this.displayName = displayName;
		this.tileX = tileX;
		this.tileY = tileY;
	}

}
