/**
 *  SceneBGM.java
 *  Created on Jun 20, 2015 6:48:31 PM for project mgne
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.scenes.commands;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.OneArgFunction;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.io.audio.BackgroundMusic;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;

/**
 * Plays a bgm from the sound manager.
 * Usage: {@code playBGM(<bgmkey>)}
 */
public class SceneBGM extends OneArgFunction {

	/**
	 * @see org.luaj.vm2.lib.OneArgFunction#call(org.luaj.vm2.LuaValue)
	 */
	@Override
	public LuaValue call(final LuaValue bgmArg) {
		SceneLib.addFunction(new SceneCommand() {
			
			BackgroundMusic bgm;
			
			/* initializer */ {
				if (bgmArg != LuaValue.NIL) {
					bgm = MGlobal.audio.generateMusicForKey(bgmArg.checkjstring());
					MGlobal.assets.loadAsset(bgm, "command bgm");
				}
			}
			
			@Override protected void internalRun() {
				if (bgm == null) {
					MGlobal.audio.fadeoutEmuBGM(1);
				} else {
					MGlobal.audio.playBGM(bgm);
				}
			}
			
		});
		return LuaValue.NIL;
	}

}
