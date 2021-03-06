/**
 *  EventEncounter.java
 *  Created on Jun 26, 2014 3:05:10 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.maps;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.maps.TiledMapObject;
import net.wombatrpgs.sagaschema.events.EventEncounterMDO;
import net.wombatrpgs.sagaschema.rpg.encounter.EncounterSetMDO;

/**
 * A block area where the player can fight some random encounters!
 */
public class EventSimpleEncounter extends EventEncounter {
	
	protected EncounterSetMDO mdo;
	
	/**
	 * Creates an encounter event region from a tiled map object. Sort of a
	 * weird mdo/object hybrid constructor?
	 * @param	mdo				The data to create from
	 * @param	object			The object to create from
	 */
	public EventSimpleEncounter(EventEncounterMDO mdo, TiledMapObject object) {
		super(object);
		this.mdo = MGlobal.data.getEntryFor(mdo.mdo, EncounterSetMDO.class);
	}
	
	/**
	 * Creates a new encounter from an encounter set MDO, implied to cover the
	 * entire map and spawned from a property.
	 * @param	mdo				The data to create from
	 */
	public EventSimpleEncounter(EncounterSetMDO mdo) {
		super();
		this.mdo = mdo;
	}

	/**
	 * @see net.wombatrpgs.saga.maps.EventEncounter#getEncounterSetForTerrain(int)
	 */
	@Override
	protected EncounterSetMDO getEncounterSetForTerrain(int terrain) {
		return mdo;
	}

}
