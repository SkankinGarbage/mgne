/**
 *  EffectTransformMDO.java
 *  Created on Jul 19, 2015 5:17:47 PM for project saga-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.rpg.warheads;

import net.wombatrpgs.mgneschema.graphics.DirMDO;
import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.SchemaLink;
import net.wombatrpgs.sagaschema.rpg.abil.data.AbilEffectMDO;

/**
 * Temporarily changes party sprite.
 */
public class EffectTransformMDO extends AbilEffectMDO {
	
	@Desc("Sprite - change to this animation")
	@SchemaLink(DirMDO.class)
	public String sprite;
	
	@Desc("Switch - this will be set while transform is active")
	public String switchName;

}
