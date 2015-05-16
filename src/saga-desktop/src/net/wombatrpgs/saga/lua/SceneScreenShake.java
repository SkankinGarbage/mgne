/**
 *  SceneScreenShake.java
 *  Created on May 15, 2015 10:11:24 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.lib.OneArgFunction;

/**
 * Shakes the screen.
 * Usage: {@code shake(seconds)}
 */
public class SceneScreenShake extends OneArgFunction {
	
	protected static final int MAX_POWER_PIXELS = 6;

	/**
	 * @see org.luaj.vm2.lib.OneArgFunction#call(org.luaj.vm2.LuaValue)
	 */
	@Override
	public LuaValue call(final LuaValue durationArg) {
		SceneLib.addFunction(new SceneCommand() {

			@Override protected void internalRun() {
				timeToWait = durationArg.tofloat();
			}

			/** @see net.wombatrpgs.mgne.scenes.SceneCommand#update(float) */
			@Override public void update(float elapsed) {
				super.update(elapsed);
				shake(MAX_POWER_PIXELS);
			}

			/** @see net.wombatrpgs.mgne.scenes.SceneCommand#finish() */
			@Override protected void finish() {
				shake(0);
				super.finish();
			}
			
			private void shake(int offsetPower) {
				int offX = 0;
				int offY = 0;
				if (offsetPower > 0) {
					offX = MGlobal.rand.nextInt(offsetPower*2) - offsetPower;
					offY = MGlobal.rand.nextInt(offsetPower*2) - offsetPower;
				}
				MGlobal.levelManager.getScreen().getCamera().shake(offX, offY);
			}
		});
		return LuaValue.NIL;
	}
	
}
