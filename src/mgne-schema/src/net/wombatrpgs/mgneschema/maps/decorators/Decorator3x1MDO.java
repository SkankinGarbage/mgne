/**
 *  Decorator1x1.java
 *  Created on Oct 13, 2013 6:13:29 AM for project MRogueSchema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgneschema.maps.decorators;

import net.wombatrpgs.mgneschema.maps.data.TileMDO;
import net.wombatrpgs.mgneschema.maps.decorators.data.SingleDecoratorMDO;
import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.InlineSchema;
import net.wombatrpgs.mgns.core.Annotations.Path;

/**
 * A decorator 3x1
 */
@Path("maps/decorators/")
public class Decorator3x1MDO extends SingleDecoratorMDO {
	
	@Desc("The left tile to replace with")
	@InlineSchema(TileMDO.class)
	public TileMDO l;
	
	@Desc("The middle tile to replace with")
	@InlineSchema(TileMDO.class)
	public TileMDO m;
	
	@Desc("The right tile to replace with")
	@InlineSchema(TileMDO.class)
	public TileMDO r;

}
