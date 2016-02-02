/**
 *  Collectable.java
 *  Created on Sep 22, 2014 11:38:09 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.items;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.maps.MapThing;
import net.wombatrpgs.sagaschema.rpg.abil.CollectableMDO;

/**
 * An item that can be collected and is not stored in the main inventory.
 */
public class Collectable {
	
	public CollectableMDO mdo;
	
	/**
	 * Creates a collectable from an mdo.
	 * @param	mdo				The MDO to create from
	 */
	public Collectable(CollectableMDO mdo) {
		this.mdo = mdo;
	}
	
	/**
	 * Creates a collectable from a key to an mdo.
	 * @param	mdoKey			The key to a CollectableMDO
	 */
	public Collectable(String mdoKey) {
		this(MGlobal.data.getEntryFor(mdoKey, CollectableMDO.class));
	}
	
	/** @return The display name of this collectable */
	public String getName() { return MGlobal.charConverter.convert(mdo.displayName); }
	
	/** @return The chest display name of this collectable */
	public String getChestName() { return MGlobal.charConverter.convert(mdo.chestName); }

	/**
	 * @see java.lang.Object#toString()
	 */
	@Override
	public String toString() {
		return mdo.key;
	}

	/**
	 * @see java.lang.Object#equals(java.lang.Object)
	 */
	@Override
	public boolean equals(Object other) {
		Collectable otherCollectable = (Collectable) other;
		if (otherCollectable == null) return false;
		return mdo.key.equals(otherCollectable.mdo.key);
	}

	/**
	 * @see java.lang.Object#hashCode()
	 */
	@Override
	public int hashCode() {
		return mdo.key.hashCode();
	}

}
