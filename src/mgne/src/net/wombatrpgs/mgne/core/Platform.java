/**
 *  Platform.java
 *  Created on Oct 30, 2013 2:53:57 AM for project mrogue-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.core;

import net.wombatrpgs.mgne.core.interfaces.Reporter;

/**
 * All the platform-specific things the engine needs. Once again, all the
 * functionality of this thing got screwed up.
 */
public abstract class Platform {
	
	/**
	 * Returns the releveant reporter for this platform. On desktop this should
	 * print to a file or something, android saves a log?
	 * @return					The reporter for the platform
	 */
	public abstract Reporter getReporter();

}
