/**
 *  EventTeleportMemoryMDO.java
 *  Created on Jul 12, 2015 4:22:01 PM for project saga-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.events;

import net.wombatrpgs.mgneschema.maps.EventMDO;
import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.Path;

/**
 * Memorizes a place to teleport back later!
 */
@Path("maps/")
public class EventTeleportMemoryMDO extends EventMDO {
	
	@Desc("Display name - displayed in the list of teleport locations")
	public String displayName;

}
