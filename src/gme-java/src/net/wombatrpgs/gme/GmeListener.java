/**
 *  GmeListener.java
 *  Created on Feb 6, 2016 1:59:03 PM for project gme-java
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.gme;

/**
 * Listens to various GME events. Mostly errors.
 */
public interface GmeListener {
	
	/**
	 * Called when an emulation error occurs. Before this is called, the track
	 * will be stopped. It probably isn't in a good state.
	 * @param	error			The error that occurred
	 */
	public void onError(String error);

}
