/**
 *  SceneItemMix.java
 *  Created on Jul 11, 2015 7:34:28 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.saga.screen.ScreenItemMix;
import net.wombatrpgs.sagaschema.rpg.mix.ItemMixSetMDO;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.OneArgFunction;

/**
 * Displays a mana-mixing screen for the given set of combinations.
 * Usage: {@code mix(<ItemMixSetMDOKey>)}
 */
public class SceneItemMix extends OneArgFunction {
	
	/**
	 * @see org.luaj.vm2.lib.OneArgFunction#call(org.luaj.vm2.LuaValue)
	 */
	@Override
	public LuaValue call(final LuaValue arg) {
		SceneLib.addFunction(new SceneCommand() {
			
			ScreenItemMix mixer;

			@Override protected void internalRun() {
				mixer = new ScreenItemMix(MGlobal.data.getEntryFor(
						arg.checkjstring(), ItemMixSetMDO.class));
				MGlobal.assets.loadAsset(mixer, "scene mixer");
				MGlobal.screens.push(mixer);
			}

			@Override protected void finish() {
				// the mixer will remove itself
				super.finish();
				mixer.dispose();
			}

			@Override protected boolean shouldFinish() {
				return super.shouldFinish() && mixer.isDone();
			}
			
		});
		
		return LuaValue.NIL;
	}

}
