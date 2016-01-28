/**
 *  InputButton.java
 *  Created on Nov 19, 2012 2:32:43 PM for project rainfall-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgneschema.io.data;

/**
 * An order that the player gives to the game via their input device. Sort of
 * meta-names for buttons rather than literal "scroll up" or "attack" or
 * whatever, though maybe that needs to be abstracted...
 */
public enum InputButton {
	
	UP					("UP"),
	DOWN				("DOWN"),
	LEFT				("LEFT"),
	RIGHT				("RIGHT"),

	BUTTON_A			("A BUTTON"),			// aka "confirm"
	BUTTON_B			("B BUTTON"),			// aka "cancel"
	BUTTON_START		("START"),				// aka "menu"
	BUTTON_SELECT		("SELECT"),				// aka who the hell uses this
	
	FULLSCREEN			(""),
	DEBUG				("");
	
	private String displayName;
	
	/**
	 * Default constructor.
	 * @param	displayName			The name to display to the user on config
	 */
	InputButton(String displayName) {
		this.displayName = displayName;
	}
	
	/** @return The display name of the button for configuration */
	public String getDisplayName() { return displayName; }

}
