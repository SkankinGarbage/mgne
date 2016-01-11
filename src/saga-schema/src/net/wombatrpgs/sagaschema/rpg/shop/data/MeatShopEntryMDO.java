/**
 *  MeatShopEntryMDO.java
 *  Created on Jan 10, 2016 2:01:35 PM for project saga-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.rpg.shop.data;

import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.HeadlessSchema;
import net.wombatrpgs.mgns.core.Annotations.SchemaLink;
import net.wombatrpgs.sagaschema.rpg.chara.CharaMDO;

/**
 * Combation monster and price
 */
public class MeatShopEntryMDO extends HeadlessSchema {
	
	@SchemaLink(CharaMDO.class)
	public String monster;
	
	@Desc("Price")
	public Integer price;

}
