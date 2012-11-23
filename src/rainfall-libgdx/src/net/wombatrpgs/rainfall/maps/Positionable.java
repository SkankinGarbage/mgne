/**
 *  Positionable.java
 *  Created on Nov 14, 2012 11:32:35 AM for project rainfall-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.rainfall.maps;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;

/**
 * Any object that can be positioned on the map and rendered at a location.
 */
public interface Positionable {
	
	/** @return The x-coord (in pixels) of this object */
	public int getX();
	/** @return The y-coord (in pixels) of this object */
	public int getY();
	
	/** @return The batch to render the associated graphics with */
	public SpriteBatch getBatch();

}
