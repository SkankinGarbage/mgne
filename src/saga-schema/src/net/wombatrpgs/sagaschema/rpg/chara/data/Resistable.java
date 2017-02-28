/**
 *  Resistable.java
 *  Created on Apr 3, 2014 12:17:41 AM for project saga-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.rpg.chara.data;

import java.util.EnumSet;

import net.wombatrpgs.sagaschema.rpg.stats.Flag;

/**
 * Damage type or status that can be resisted.
 */
public interface Resistable {
	
	/**
	 * Returns all flags used to resist this status or damage type. During
	 * damage calculation, this function is called, each flag status is checked,
	 * and appropriate action taken. Return empty set for no possible resist.
	 * Should return the most specific elements first, ie, prioritize element
	 * resist over damage resist. This is useful for backwards correlation.
	 * 
	 * Resistances only partially block some of the damage.
	 * 
	 * @return					The stats that indicate resistance
	 */
	public EnumSet<Flag> getResistFlags();
	
	/**
	 * Same as resistances, but these flags grant complete immunity.
	 * 
	 * @return					The stats that indicate immunity
	 */
	public EnumSet<Flag> getImmuneFlags();
	
	/**
	 * Returns the flags that indicate a creature is weak to this effect, or
	 * empty for no weakness flags. Should return the most specific elements
	 * first.
	 * @return					The stats that indicate weakness
	 */
	public EnumSet<Flag> getWeakFlags();

}
