/**
 *  CollectableSet.java
 *  Created on Sep 22, 2014 11:41:33 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.items;

import java.util.HashMap;
import java.util.Map;

import com.fasterxml.jackson.annotation.JsonIgnore;

import net.wombatrpgs.mgne.core.MGlobal;

/**
 * A bunch of collectables. Ensure this remains a POJO as it's serialized in
 * JSON directly.
 */
public class CollectableSet {
	
	public Map<Collectable, Integer> quantities;
	
	/**
	 * Creates an empty collectable set.
	 */
	public CollectableSet() {
		quantities = new HashMap<Collectable, Integer>();
	}
	
	/**
	 * Adds a single collectable to the collection.
	 * @param	collectable		The collectable to add
	 */
	public void addCollectable(Collectable collectable) {
		Integer quantity = quantities.get(collectable);
		if (quantity == null) quantity = 0;
		quantities.put(collectable, quantity + 1);
	}
	
	/**
	 * Adds a single collectable based on its data key.
	 * @param	mdoKey			The key of the mdo of the collectable to add
	 */
	public void addCollectable(String mdoKey) {
		addCollectable(new Collectable(mdoKey));
	}
	
	/**
	 * Copies the value of a collectable in one collection to this one.
	 * @param	collection		The collection to copy value from
	 * @param	collectable		The collectable to copy the value of
	 */
	public void copyCollectableValue(CollectableSet collection, Collectable collectable) {
		quantities.put(collectable, collection.getQuantity(collectable));
	}
	
	/**
	 * Removes a single collectable from the collection.
	 * @param	collectable		A collectable to remove
	 */
	public void removeCollectable(Collectable collectable) {
		subtractCollectable(collectable, 1);
	}
	
	/**
	 * Checks the quantity of a collectable in the collection.
	 * @param	collectable		The collectable to check quantity of
	 * @return					The number of that collectable in stock
	 */
	@JsonIgnore
	public int getQuantity(Collectable collectable) {
		Integer quantity = quantities.get(collectable);
		return (quantity == null) ? 0 : quantity;
	}
	
	/**
	 * Checks the quantity of a collectable in the collection with the given
	 * key.
	 * @param	collectableKey	The key of the collectable to check
	 * @return					The number of that collectable in stock
	 */
	@JsonIgnore
	public int getQuantity(String collectableKey) {
		return getQuantity(new Collectable(collectableKey));
	}
	
	/**
	 * Removes some collectables from the set.
	 * @param	collectable		The collectable to remove
	 * @param	quantity		How many of that type to remove
	 */
	public void subtractCollectable(Collectable collectable, int quantity) {
		int haveQuantity = quantities.get(collectable);
		if (haveQuantity >= quantity) {
			int result = haveQuantity - quantity;
			if (result == 0) {
				quantities.remove(collectable);
			} else {
				quantities.put(collectable, result);
			}
		} else {
			MGlobal.reporter.warn("Not enough collectable: " + collectable);
		}
	}
	
	/**
	 * Removes some collectables from the set by key.
	 * @param	collectableKey	The key of the CollectableItem to remove
	 * @param	quantity		How many of that type to remove
	 */
	public void subtractCollectable(String collectableKey, int quantity) {
		subtractCollectable(new Collectable(collectableKey), quantity);
	}
	
	/**
	 * Converts the collectables map into a set of name/quantity pairings. This
	 * is usually used for display.
	 * @return					A map of name to quantity
	 */
	public Map<String, Integer> toNameQuantityPairs() {
		Map<String, Integer> pairs = new HashMap<String, Integer>();
		for (Collectable key : quantities.keySet()) {
			pairs.put(key.getName(), getQuantity(key));
		}
		return pairs;
	}
	
	/**
	 * Counts the number of unique items appearing in this collection.
	 * @return					The number of unique items contained
	 */
	@JsonIgnore
	public int getTypeCount() {
		return quantities.size();
	}

}
