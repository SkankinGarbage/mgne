/**
 *  SceneScreenShake.java
 *  Created on May 15, 2015 10:11:24 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.maps.Level;
import net.wombatrpgs.mgne.maps.events.MapEvent;
import net.wombatrpgs.mgne.maps.objects.TimerListener;
import net.wombatrpgs.mgne.maps.objects.TimerObject;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.Varargs;
import org.luaj.vm2.lib.VarArgFunction;

/**
 * Shakes the screen.
 * Usage: {@code shake(seconds, <power_pixels>)}
 */
public class SceneScreenShake extends VarArgFunction {
	
	protected static final int MAX_POWER_PIXELS = 6;

	/**
	 * @see org.luaj.vm2.lib.VarArgFunction#invoke(org.luaj.vm2.Varargs)
	 */
	@Override
	public Varargs invoke(final Varargs args) {
		SceneLib.addFunction(new SceneCommand() {
			
			int power;
			Level map;
			MapEvent event;

			@Override protected void internalRun() {
				if (args.narg() >= 2) {
					power = args.arg(2).checkint();
				} else {
					power = MAX_POWER_PIXELS;
				}
				if (args.narg() >= 3 && args.arg(3).checkboolean()) {
					timeToWait = args.arg(1).tofloat();
				}
				
				map = MGlobal.levelManager.getActive();
				event = new MapEvent() {
					@Override public void update(float elapsed) {
						super.update(elapsed);
						shake(power);
					}
				};
				map.addEvent(event);
				
				new TimerObject(args.arg(1).tofloat(), parent.getScreen(), new TimerListener() {
					@Override public void onTimerZero(TimerObject source) {
						map.removeEvent(event);
						shake(0);
					}
				});
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
