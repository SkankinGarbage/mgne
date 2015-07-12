/**
 *  MixEntryMDO.java
 *  Created on Jul 11, 2015 8:00:53 PM for project saga-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.rpg.mix.data;

import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.HeadlessSchema;
import net.wombatrpgs.mgns.core.Annotations.SchemaLink;
import net.wombatrpgs.sagaschema.rpg.abil.CollectableMDO;

/**
 * Ingredient in a mix.
 */
public class MixEntryMDO extends HeadlessSchema {
	
	@SchemaLink(CollectableMDO.class)
	public String item;
	
	@Desc("Quantity")
	public Integer quantity;

}
