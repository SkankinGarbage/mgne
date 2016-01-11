/**
 *  MeatInventoryMDO.java
 *  Created on Jan 10, 2016 1:59:39 PM for project saga-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.rpg.shop;

import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.Path;
import net.wombatrpgs.mgns.core.MainSchema;
import net.wombatrpgs.mgns.core.Annotations.InlineSchema;
import net.wombatrpgs.sagaschema.rpg.shop.data.MeatShopEntryMDO;

/**
 * Collection of meats and prices.
 */
@Path("rpg/")
public class MeatShopMDO extends MainSchema {
	
	@Desc("Shop inventory")
	@InlineSchema(MeatShopEntryMDO.class)
	public MeatShopEntryMDO[] entries;

}
