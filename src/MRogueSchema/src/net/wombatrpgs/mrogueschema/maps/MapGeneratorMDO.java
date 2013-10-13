/**
 *  MapGenerator.java
 *  Created on Oct 4, 2013 1:30:51 AM for project MRogueSchema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mrogueschema.maps;

import net.wombatrpgs.mgns.core.Annotations.DefaultValue;
import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.InlineSchema;
import net.wombatrpgs.mgns.core.Annotations.SchemaLink;
import net.wombatrpgs.mgns.core.MainSchema;
import net.wombatrpgs.mgns.core.Annotations.Path;
import net.wombatrpgs.mrogueschema.maps.data.GenerationType;
import net.wombatrpgs.mrogueschema.maps.data.WallTilesMDO;
import net.wombatrpgs.mrogueschema.maps.decorators.data.DecoratorMDO;

/**
 * Something to procedurally generate maps.
 */
@Path("maps/")
public class MapGeneratorMDO extends MainSchema {
	
	@Desc("Floor tile(s)")
	@InlineSchema(TileMDO.class)
	public TileMDO floorTiles;

	@Desc("Wall tile(s)")
	@SchemaLink(WallTilesMDO.class)
	public String wallTiles;
	
	@Desc("Ceiling tile(s)")
	@InlineSchema(TileMDO.class)
	public TileMDO ceilingTiles;
	
	@Desc("Generation algorithm")
	public GenerationType generator;
	
	@Desc("Minimum room width, in tiles")
	public Integer minRoomWidth;
	
	@Desc("Minimum room height, in tiles")
	public Integer minRoomHeight;
	
	@Desc("Maximum room width, in tiles")
	public Integer maxRoomWidth;
	
	@Desc("Maximum room height, in tiles")
	public Integer maxRoomHeight;
	
	@Desc("Room density - what percentage of the map should be taken up by "
			+ "rooms, 0.0 to 1.0")
	@DefaultValue(".3")
	public Float density;
	
	@Desc("Connectivity - used in some heuristics, may change later, but right "
			+ "now 0.0 is unconnected rooms, 1.0 is (roughly) minimal "
			+ "connected rooms, and anything more has redundencies.")
	@DefaultValue("1.0")
	public Float connectivity;
	
	@Desc("Decorators - things that go through after the map is generated "
			+ "and add special effects")
	@SchemaLink(DecoratorMDO.class)
	public String[] decorators;

}
