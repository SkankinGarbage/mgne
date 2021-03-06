/**
 *  SceneRecruit.java
 *  Created on Jun 16, 2014 1:48:51 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.saga.screen.ScreenRecruit;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.OneArgFunction;

/**
 * Recruit and name a character to the party.
 */
public class SceneRecruit extends OneArgFunction {

	/**
	 * @see org.luaj.vm2.lib.OneArgFunction#call(org.luaj.vm2.LuaValue)
	 */
	@Override
	public LuaValue call(final LuaValue arg) {
		SceneLib.addFunction(new SceneCommand() {

			ScreenRecruit recruitScreen;

			@Override protected void internalRun() {
				recruitScreen = new ScreenRecruit(arg.checkjstring());
				MGlobal.assets.loadAsset(recruitScreen, "recruit screen");
				MGlobal.screens.push(recruitScreen);
			}

			@Override protected void finish() {
				super.finish();
				recruitScreen.dispose();
			}

			@Override protected boolean shouldFinish() {
				return super.shouldFinish() && recruitScreen.isDone();
			}
			
		});
		
		return LuaValue.NIL;
	}

}
