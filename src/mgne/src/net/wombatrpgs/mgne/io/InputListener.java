/**
 *  InputListener.java
 *  Created on Jan 24, 2016 8:36:10 PM for project mgne
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.io;

/**
 * Listener for raw key input events
 */
public interface InputListener {
	
	/**
	 * Called when a key is depressed.
	 * @param	keycode			The code of the pressed key
	 */
	public void onKeyDown(int keycode);
	
	/**
	 * Called when a key is upressed.
	 * @param	keycode			The code of the unpressed key
	 */
	public void onKeyUp(int keycode);

}
