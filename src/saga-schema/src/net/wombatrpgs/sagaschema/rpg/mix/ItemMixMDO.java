/**
 *  ItemMixMDO.java
 *  Created on Jul 11, 2015 6:51:26 PM for project saga-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.rpg.mix;

import net.wombatrpgs.mgns.core.Annotations.InlineSchema;
import net.wombatrpgs.mgns.core.MainSchema;
import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.Path;
import net.wombatrpgs.mgns.core.Annotations.SchemaLink;
import net.wombatrpgs.sagaschema.rpg.abil.CombatItemMDO;
import net.wombatrpgs.sagaschema.rpg.mix.data.MixEntryMDO;

/**
 * Represents a combination of two collectables to form a mana-mixed item.
 */
@Path("rpg/")
public class ItemMixMDO extends MainSchema {
	
	@Desc("Ingredients")
	@InlineSchema(MixEntryMDO.class)
	public MixEntryMDO[] ingredients;
	
	@Desc("The resulting combat item")
	@SchemaLink(CombatItemMDO.class)
	public String result;

}
