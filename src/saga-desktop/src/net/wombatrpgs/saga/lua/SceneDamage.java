/**
 *  SceneDamage.java
 *  Created on May 25, 2015 12:50:05 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.chara.Chara;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.OneArgFunction;

/**
 * Inflicts set damage on party members.
 * Usage: {@code damage(<amt>)}
 */
public class SceneDamage extends OneArgFunction {

	/**
	 * @see org.luaj.vm2.lib.OneArgFunction#call(org.luaj.vm2.LuaValue)
	 */
	@Override
	public LuaValue call(final LuaValue damageArg) {
		SceneLib.addFunction(new SceneCommand() {

			@Override protected void internalRun() {
				int damage = damageArg.checkint();
				for (Chara hero : SGlobal.heroes.getAll()) {
					if (hero.isAlive()) {
						hero.damage(damage, false);
						if (hero.isDead()) {
							hero.heal(1);
						}
					}
				}
			}
			
		});
		return LuaValue.NIL;
	}
	
}
