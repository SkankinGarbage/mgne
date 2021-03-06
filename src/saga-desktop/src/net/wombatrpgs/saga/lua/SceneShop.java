/**
 *  SceneShop.java
 *  Created on May 20, 2014 2:21:46 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import java.util.ArrayList;
import java.util.List;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.saga.screen.ScreenItemShop;
import net.wombatrpgs.sagaschema.rpg.abil.CombatItemMDO;
import net.wombatrpgs.sagaschema.rpg.shop.ShopMDO;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.Varargs;
import org.luaj.vm2.lib.VarArgFunction;

/**
 * A shop command! Arguments are the mdos keys of the items for sale.
 */
public class SceneShop extends VarArgFunction {
	
	/**
	 * @see org.luaj.vm2.lib.VarArgFunction#invoke(org.luaj.vm2.Varargs)
	 */
	@Override
	public Varargs invoke(final Varargs args) {
		SceneLib.addFunction(new SceneCommand() {
			
			ScreenItemShop shop;

			@Override protected void internalRun() {
				if (args.narg() > 1) {
					List<CombatItemMDO> mdos = new ArrayList<CombatItemMDO>();
					for (int i = 1; i <= args.narg(); i += 1) {
						String key = args.checkstring(i).checkjstring();
						mdos.add(MGlobal.data.getEntryFor(key, CombatItemMDO.class));
					}
					shop = new ScreenItemShop(mdos);
				} else {
					String key = args.arg(1).checkjstring();
					shop = new ScreenItemShop(MGlobal.data.getEntryFor(key, ShopMDO.class));
				}
				MGlobal.assets.loadAsset(shop, "scene shop");
				MGlobal.screens.push(shop);
			}

			@Override protected void finish() {
				// the inn will remove itself
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
