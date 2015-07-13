/**
 *  SagaEventFactory.java
 *  Created on Jun 26, 2014 12:51:17 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.maps;

import com.badlogic.gdx.maps.tiled.TiledMap;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.maps.TiledMapObject;
import net.wombatrpgs.mgne.maps.events.EventFactory;
import net.wombatrpgs.mgne.maps.events.MapEvent;
import net.wombatrpgs.sagaschema.events.EventCeilingMDO;
import net.wombatrpgs.sagaschema.events.EventChestMDO;
import net.wombatrpgs.sagaschema.events.EventDoorMDO;
import net.wombatrpgs.sagaschema.events.EventEncounterMDO;
import net.wombatrpgs.sagaschema.events.EventTeleportMemoryMDO;
import net.wombatrpgs.sagaschema.rpg.encounter.EncounterSetMDO;
import net.wombatrpgs.sagaschema.rpg.encounter.TerrainEncounterSetMDO;

/**
 * Saga-specific map events.
 */
public class SagaEventFactory extends EventFactory {
	
	protected static final String TYPE_ENCOUNTER = "Encounter";
	protected static final String TYPE_CEILING = "Ceiling";
	protected static final String TYPE_CHEST = "Chest";
	protected static final String TYPE_DOOR = "Door";
	protected static final String TYPE_TELEPORT_MEMORY = "Teleport Memory";
	
	protected static final String PROPERTY_ENCOUNTER = "encounter";
	protected static final String PROPERTY_TERRAIN_ENCOUNTER = "terrainEncounter";

	/**
	 * @see net.wombatrpgs.mgne.maps.events.EventFactory#createEvent
	 * (net.wombatrpgs.mgne.maps.TiledMapObject)
	 */
	@Override
	protected MapEvent createEvent(TiledMapObject object) {
		String type = object.getString(TiledMapObject.PROPERTY_TYPE);
		if (TYPE_ENCOUNTER.equals(type)) {
			return new EventSimpleEncounter(object.generateMDO(EventEncounterMDO.class), object);
		} else if (TYPE_CEILING.equals(type)) {
			return new EventCeiling(object.generateMDO(EventCeilingMDO.class), object);
		} else if (TYPE_CHEST.equals(type)) {
			return new EventChest(object.generateMDO(EventChestMDO.class), object);
		} else if (TYPE_DOOR.equals(type)) {
			return new EventDoor(object.generateMDO(EventDoorMDO.class));
		} else if (TYPE_TELEPORT_MEMORY.equals(type)) {
			return new EventTeleportMemory(object.generateMDO(EventTeleportMemoryMDO.class));
		}
		return super.createEvent(object);
	}

	/**
	 * @see net.wombatrpgs.mgne.maps.events.EventFactory#createFromMapProperty
	 * (com.badlogic.gdx.maps.tiled.TiledMap, java.lang.String, java.lang.String)
	 */
	@Override
	public MapEvent createFromMapProperty(TiledMap map, String key, String value) {
		MapEvent superResult = super.createFromMapProperty(map, key, value);
		if (superResult != null) return superResult;
		if (PROPERTY_ENCOUNTER.equals(key)) {
			EncounterSetMDO mdo = MGlobal.data.getEntryFor(value, EncounterSetMDO.class);
			return new EventSimpleEncounter(mdo);
		} else if (PROPERTY_TERRAIN_ENCOUNTER.equals(key)) {
			TerrainEncounterSetMDO mdo = MGlobal.data.getEntryFor(value, TerrainEncounterSetMDO.class);
			return new EventTerrainEncounter(mdo, map);
		}
		return null;
	}

}
