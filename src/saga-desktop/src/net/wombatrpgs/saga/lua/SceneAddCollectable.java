/**
 *  SceneAddItem.java
 *  Created on Sep 13, 2014 9:32:04 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.saga.core.SGlobal;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.OneArgFunction;

/**
 * Adds a collectable item to the party inventory.
 * Usage: {@code addItem(<collectablekey>)}
 */
public class SceneAddCollectable extends OneArgFunction {

	/**
	 * @see org.luaj.vm2.lib.OneArgFunction#call(org.luaj.vm2.LuaValue)
	 */
	@Override
	public LuaValue call(final LuaValue itemArg) {
		SceneLib.addFunction(new SceneCommand() {

			@Override protected void internalRun() {
				String key = itemArg.checkjstring();
				SGlobal.heroes.getCollection().addCollectable(key);
			}
			
		});
		return LuaValue.NIL;
	}

}
