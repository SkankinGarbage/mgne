/**
 *  CondChance.java
 *  Created on Oct 27, 2013 7:22:12 AM for project mrogue-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.ai.cond;

import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.CharacterEvent;
import net.wombatrpgs.saga.rpg.ai.BTCondition;

/**
 * Conditional based on the RNG.
 */
public class CondChance extends BTCondition {
	
	protected float chance;

	/**
	 * Creates a variable probability condition.
	 * @param	actor			The actor who might take action
	 * @param	chance			The chance this will return true (0-1)
	 */
	public CondChance(CharacterEvent actor, float chance) {
		super(actor);
		this.chance = chance;
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.ai.BTCondition#isMet()
	 */
	@Override
	protected boolean isMet() {
		return SGlobal.rand.nextFloat() <= chance;
	}

}
