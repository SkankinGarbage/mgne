/**
 *  EventTeleportMemory.java
 *  Created on Jul 12, 2015 4:23:46 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.maps;

import net.wombatrpgs.mgne.maps.Level;
import net.wombatrpgs.mgne.maps.events.MapEvent;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.sagaschema.events.EventTeleportMemoryMDO;

/**
 * Memorizes a location so you can teleport back to it!
 */
public class EventTeleportMemory extends MapEvent {
	
	protected EventTeleportMemoryMDO mdo;

	public EventTeleportMemory(EventTeleportMemoryMDO mdo) {
		super(mdo);
		this.mdo = mdo;
	}

	/**
	 * @see net.wombatrpgs.mgne.maps.events.MapEvent#onMapFocusGained
	 * (net.wombatrpgs.mgne.maps.Level)
	 */
	@Override
	public void onMapFocusGained(Level map) {
		super.onMapFocusGained(map);
		String key = map.getKeyName();
		for (TeleportMemory memory : SGlobal.teleports) {
			if (memory.mapKey.equals(key)) {
				// already added
				return;
			}
		}
		SGlobal.teleports.add(new TeleportMemory(key,
				mdo.displayName,
				getTileX(), 
				parent.getHeight() - getTileY() - 1));
	}

}
