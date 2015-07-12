/**
 *  ItemMixSetMDO.java
 *  Created on Jul 11, 2015 6:53:49 PM for project saga-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.rpg.mix;

import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.Path;
import net.wombatrpgs.mgns.core.Annotations.SchemaLink;
import net.wombatrpgs.mgns.core.MainSchema;

/**
 * A list of combinations that are all accessed from the same interface or shop.
 */
@Path("rpg/")
public class ItemMixSetMDO extends MainSchema {
	
	@Desc("The available combinations")
	@SchemaLink(ItemMixMDO.class)
	public String[] combinations;

}
