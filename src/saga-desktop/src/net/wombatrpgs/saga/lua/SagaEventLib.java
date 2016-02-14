/**
 *  SagaEventLib.java
 *  Created on Aug 27, 2014 12:26:34 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.items.CollectableSet;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.OneArgFunction;
import org.luaj.vm2.lib.TwoArgFunction;
import org.luaj.vm2.lib.ZeroArgFunction;
import org.luaj.vm2.lib.jse.CoerceJavaToLua;

/**
 * Crappy library with utility calls for event scripting.
 */
public class SagaEventLib extends TwoArgFunction {

	/**
	 * @see org.luaj.vm2.lib.TwoArgFunction#call
	 * (org.luaj.vm2.LuaValue, org.luaj.vm2.LuaValue)
	 */
	@Override
	public LuaValue call(LuaValue modname, LuaValue env) {
		LuaValue library = tableOf();
		
		env.set("getHero", new OneArgFunction() {
			@Override public LuaValue call(LuaValue slotArg) {
				int slot = slotArg.checkint();
				if (slot >= SGlobal.heroes.size()) {
					return LuaValue.NIL;
				} else {
					return SGlobal.heroes.getStoryHero(slot).toLua();
				}
			}
		});
		
		env.set("getAvatar", new ZeroArgFunction() {
			@Override public LuaValue call() {
				return MGlobal.getHero().toLua();
			}
		});
		
		env.set("hasItem", new OneArgFunction() {
			@Override public LuaValue call(LuaValue itemArg) {
				String key = itemArg.checkjstring();
				return SGlobal.heroes.isCarryingItemType(key) ? LuaValue.TRUE : LuaValue.FALSE;
			}
		});
		
		env.set("hasCollectable", new OneArgFunction() {
			@Override public LuaValue call(LuaValue collectableArg) {
				String key = collectableArg.checkjstring();
				CollectableSet set = SGlobal.heroes.getCollection();
				return set.getQuantity(key) > 0 ? LuaValue.TRUE : LuaValue.FALSE;
			}
		});
		
		env.set("collectableCount", new OneArgFunction() {
			@Override public LuaValue call(LuaValue collectableArg) {
				String key = collectableArg.checkjstring();
				CollectableSet set = SGlobal.heroes.getCollection();
				return CoerceJavaToLua.coerce(set.getQuantity(key));
			}
		});
		
		env.set("playScene", new ZeroArgFunction() {
			@Override public LuaValue call() {
				SceneLib.runExtraCommands();
				return LuaValue.NIL;
			}
		});
		
		env.set("inventoryFull", new ZeroArgFunction() {
			@Override public LuaValue call() {
				return CoerceJavaToLua.coerce(SGlobal.heroes.getInventory().isFull());
			}
		});
		
		env.set("sagaeventlib", library);
		return library;
	}

}
