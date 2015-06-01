/**
 *  EffectReviveMDO.java
 *  Created on May 31, 2015 9:04:32 PM for project saga-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.rpg.warheads;

import net.wombatrpgs.mgns.core.Annotations.DefaultValue;
import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.Nullable;
import net.wombatrpgs.sagaschema.rpg.stats.Stat;

/**
 * Levitates nearest corpse.
 */
public class EffectReviveMDO extends EffectAllyTargetMDO {
	
	@Desc("Heal base - after resurrection, targets starts with this much health")
	@DefaultValue("0")
	public Integer base;
	
	@Desc("Heal power - multiplier for the heal (added to base)")
	@DefaultValue("0")
	public Integer power;
	
	@Desc("Power stat - this stat is quartered and multiplied by power")
	@DefaultValue("None")
	@Nullable
	public Stat powerStat;

}
