/**
 *  SceneRemoveItem.java
 *  Created on Jul 4, 2015 6:35:43 PM for project saga-desktop
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
 * Removes an item from the party inventory. No effect if item isn't found or if
 * it's equipped to someone.
 * Usage: {@code removeItem(<itemkey>)}
 */
public class SceneRemoveItem extends OneArgFunction {

	/**
	 * @see org.luaj.vm2.lib.OneArgFunction#call(org.luaj.vm2.LuaValue)
	 */
	@Override
	public LuaValue call(final LuaValue itemArg) {
		SceneLib.addFunction(new SceneCommand() {

			@Override protected void internalRun() {
				String key = itemArg.checkjstring();
				SGlobal.heroes.getInventory().removeItemByKey(key);
			}
			
		});
		return LuaValue.NIL;
	}

}
