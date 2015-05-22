/**
 *  SceneHeal.java
 *  Created on May 21, 2015 11:46:59 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.chara.Chara;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.ZeroArgFunction;

/**
 * Full heals the party.
 * Usage: {@code heal()}
 */
public class SceneHeal extends ZeroArgFunction {

	/**
	 * @see org.luaj.vm2.lib.ZeroArgFunction#call()
	 */
	@Override
	public LuaValue call() {
		SceneLib.addFunction(new SceneCommand() {

			@Override protected void internalRun() {
				for (Chara hero : SGlobal.heroes.getAll()) {
					hero.restoreHP();
				}
			}
			
		});

		return LuaValue.NIL;
	}

}
