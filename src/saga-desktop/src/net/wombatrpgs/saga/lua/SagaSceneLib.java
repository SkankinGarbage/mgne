/**
 *  SagaSceneLib.java
 *  Created on Apr 19, 2014 1:25:54 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import net.wombatrpgs.mgne.scenes.commands.SceneChoice;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.TwoArgFunction;

/**
 * Library for Saga-specific scene calls.
 */
public class SagaSceneLib extends TwoArgFunction {

	/**
	 * @see org.luaj.vm2.lib.TwoArgFunction#call
	 * (org.luaj.vm2.LuaValue,org.luaj.vm2.LuaValue)
	 */
	@Override
	public LuaValue call(LuaValue modname, LuaValue env) {
		LuaValue library = tableOf();
		
		env.set("battle", new SceneBattle());
		env.set("inn", new SceneInn());
		env.set("shop", new SceneShop());
		env.set("recruit", new SceneRecruit());
		env.set("name", new SceneName());
		env.set("fade", new SceneFade());
		env.set("addMember", new SceneAddMember());
		env.set("removeMember", new SceneRemoveMember());
		env.set("reset", new SceneReset());
		env.set("addItem", new SceneAddItem());
		env.set("shake", new SceneScreenShake());
		env.set("memTele", new SceneMemoryTeleport());
		env.set("heal", new SceneHeal());
		env.set("damage", new SceneDamage());
		env.set("choice", new SceneChoice());
		
		env.set("sagalib", library);
		return library;
	}

}
