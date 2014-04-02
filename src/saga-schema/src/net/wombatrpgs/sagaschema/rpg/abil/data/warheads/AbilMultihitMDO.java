/**
 *  AbilMultihit.java
 *  Created on Apr 1, 2014 2:23:53 AM for project saga-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.rpg.abil.data.warheads;

import net.wombatrpgs.mgns.core.Annotations.DefaultValue;
import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.Nullable;
import net.wombatrpgs.sagaschema.rpg.abil.data.AbilEffectMDO;
import net.wombatrpgs.sagaschema.rpg.data.Stat;

/**
 * Multi-hit attack (2pincer, 3head, etc)
 */
public class AbilMultihitMDO extends AbilEffectMDO {
	
	@Desc("Base power - base multiplier of each hit")
	public Integer power;
	
	@Desc("First hits - how many times this attack hits on first use")
	public Integer hitsFirst;
	
	@Desc("Successive hits - how many times this attack hits on other use")
	public Integer hitsLast;
	
	@Desc("Attack stat - this stat is quartered and then multiplied by power")
	@DefaultValue("STR")
	@Nullable
	public Stat attackStat;
	
	@Desc("Defend stat - this stat is subtracted from incoming damage")
	@DefaultValue("DEF")
	@Nullable
	public Stat defendStat;

}