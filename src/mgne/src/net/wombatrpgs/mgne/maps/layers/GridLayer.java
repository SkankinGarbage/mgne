/**
 *  GridLayer.java
 *  Created on Jan 8, 2014 4:06:26 PM for project saga
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.maps.layers;

import net.wombatrpgs.mgne.maps.Level;

/**
 * A layer of tiles that is part of a level. It's named "grid" so as to not
 * conflict with the stubby libgdx idea of a TiledLayer which isn't a layer at
 * all, really. This is split into two halves, a generated grid layer and a
 * loaded grid layer. Generated grid layers run off a tile grid, and the loaded
 * ones just make reference to the tiled layer that spawned them. This class
 * holds their common functionality.
 */
public abstract class GridLayer extends Layer implements Comparable<GridLayer> {
	
	protected float z;
	
	/**
	 * Creates a new grid layer of any type.
	 * @param	parent			The parent level of the layer
	 * @param	z				The z-float of this layer, follows some weird
	 * 							fractional for upper chip, whole for lower chip
	 * 							rules that should really be explained somewhere	
	 */
	public GridLayer(Level parent, float z) {
		super(parent);
		this.z = z;
	}
	
	/**
	 * @see net.wombatrpgs.mgne.maps.layers.Layer#isLowerChip()
	 */
	@Override
	public boolean isLowerChip() {
		return Math.floor(z) == z;
	}
	
	/**
	 * Checks if the tile at the given location has the given property.
	 * @param	tileX			The x-coord to check (in tiles)
	 * @param	tileY			The y-coord to check (in tiles)
	 * @param	property		The property to check for
	 * @return					True if that property exists there
	 */
	public abstract boolean hasPropertyAt(int tileX, int tileY, String property);
	
	/**
	 * Gets the value of a given property key at the given tile.
	 * @param	tileX			The checked x-coord (in tiles)
	 * @param	tileY			The checked y-coord (in tiles)
	 * @param	property		The name of the property to check
	 * @return					The value of that property, or null if none
	 */
	public abstract String getPropertyAt(int tileX, int tileY, String property);
	
	/**
	 * Fetches the terrain ID (defined in Tiled usually) of the tile located
	 * at the given coordiantes.
	 * @param	tileX			The location to check (in tiles)
	 * @param	tileY			The location to check (in tiles)
	 * @return					The terrain ID at that location
	 */
	public abstract int getTerrainAt(int tileX, int tileY);
	
	/**
	 * @see java.lang.Comparable#compareTo(java.lang.Object)
	 */
	@Override
	public int compareTo(GridLayer other) {
		return (int) ((getZ() * 4) - (other.getZ() * 4));
	}

	/**
	 * Gets the z-value of the layer. Layers with the same z-value share
	 * collisions and collision detection. 0 represents the floor, and each
	 * subsequent integer is another floor.
	 * @return					The z-value (depth) of this layer
	 */
	public float getZ() {
		return z;
	}

}
