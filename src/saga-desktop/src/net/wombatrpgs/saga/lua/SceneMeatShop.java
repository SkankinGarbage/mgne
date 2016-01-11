/**
 *  SceneMeatShop.java
 *  Created on Jan 10, 2016 2:21:46 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.saga.screen.ScreenMeatShop;
import net.wombatrpgs.sagaschema.rpg.shop.MeatShopMDO;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.Varargs;
import org.luaj.vm2.lib.VarArgFunction;

/**
 * A meat shop command! Arguments are the mdos keys of the meat for sale.
 */
public class SceneMeatShop extends VarArgFunction {
	
	/**
	 * @see org.luaj.vm2.lib.VarArgFunction#invoke(org.luaj.vm2.Varargs)
	 */
	@Override
	public Varargs invoke(final Varargs args) {
		SceneLib.addFunction(new SceneCommand() {
			
			ScreenMeatShop shop;

			@Override protected void internalRun() {
				String key = args.arg(1).checkjstring();
				shop = new ScreenMeatShop(MGlobal.data.getEntryFor(key, MeatShopMDO.class));
				MGlobal.assets.loadAsset(shop, "scene shop");
				MGlobal.screens.push(shop);
			}

			@Override protected void finish() {
				// the shop will remove itself
				super.finish();
				shop.dispose();
			}

			@Override protected boolean shouldFinish() {
				return super.shouldFinish() && shop.isDone();
			}
			
		});
		
		return LuaValue.NIL;
	}
}
