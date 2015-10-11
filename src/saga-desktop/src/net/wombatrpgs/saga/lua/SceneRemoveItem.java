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
import org.luaj.vm2.Varargs;
import org.luaj.vm2.lib.VarArgFunction;

/**
 * Removes an item from the party inventory. No effect if item isn't found or if
 * it's equipped to someone.
 * Usage: {@code removeItem(<itemkey>, <isCollectable=false)}
 */
public class SceneRemoveItem extends VarArgFunction {

	/**
	 * @see org.luaj.vm2.lib.VarArgFunction#invoke(org.luaj.vm2.Varargs)
	 */
	@Override
	public Varargs invoke(Varargs args) {
		
		final String key = args.arg(1).checkjstring();
		final boolean isCollectable = args.narg() >= 2 ? args.arg(2).checkboolean() : false;
		
		SceneLib.addFunction(new SceneCommand() {
			@Override protected void internalRun() {
				if (isCollectable) {
					SGlobal.heroes.getCollection().removeCollectable(key);
				} else {
					SGlobal.heroes.getInventory().removeItem(key);
				}
			}	
		});
		return LuaValue.NIL;
	}

}
