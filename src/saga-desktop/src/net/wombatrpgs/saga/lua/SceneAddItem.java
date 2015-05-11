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
import net.wombatrpgs.saga.rpg.items.CombatItem;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.OneArgFunction;

/**
 * Adds an item to the party inventory if not full.
 * Usage: {@code addItem(<itemkey>)}
 */
public class SceneAddItem extends OneArgFunction {

	/**
	 * @see org.luaj.vm2.lib.OneArgFunction#call(org.luaj.vm2.LuaValue)
	 */
	@Override
	public LuaValue call(final LuaValue itemArg) {
		SceneLib.addFunction(new SceneCommand() {

			@Override protected void internalRun() {
				String key = itemArg.checkjstring();
				CombatItem item = new CombatItem(key);
				if (!SGlobal.heroes.getInventory().isFull()) {
					SGlobal.heroes.addItem(item);
				}
			}
			
		});
		return LuaValue.NIL;
	}

}
