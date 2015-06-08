/**
 *  SceneTargetTele.java
 *  Created on Jun 7, 2015 4:25:23 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.scenes.commands;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.FinishListener;
import net.wombatrpgs.mgne.maps.Level;
import net.wombatrpgs.mgne.maps.events.MapEvent;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.mgneschema.maps.data.OrthoDir;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.Varargs;
import org.luaj.vm2.lib.VarArgFunction;

/**
 * Teleports hero to an event on another map
 * Usage: {@code teleport(<mapname>, <targeteventname>, [direction])}
 */
public class SceneTargetTele extends VarArgFunction {

	/**
	 * @see org.luaj.vm2.lib.VarArgFunction#invoke(org.luaj.vm2.Varargs)
	 */
	@Override
	public Varargs invoke(final Varargs args) {
		SceneLib.addFunction(new SceneCommand() {
			
			String mapName;
			String eventName;
			OrthoDir dir;
			boolean teleportFinished;
			
			/* Initializer */ {
				mapName = args.arg(1).checkjstring();
				eventName = args.arg(2).checkjstring();
				dir = (args.narg() >= 3) ? OrthoDir.valueOf(args.arg(3).checkjstring()) : null;
				teleportFinished = false;
			}

			@Override protected void internalRun() {
				final Level map = MGlobal.levelManager.getLevel(mapName);
				MapEvent target = map.getEventByName(eventName);
				if (target == null) {
					MGlobal.reporter.err("No event named " + eventName + " on " + map);
				}
				int tileX = target.getTileX();
				int tileY = map.getHeight() - target.getTileY() - 1;
				FinishListener onFinish = new FinishListener() {
					@Override public void onFinish() {
						teleportFinished = true;
						map.update(0);
					}
				};
				MGlobal.levelManager.getTele().teleport(mapName,
						tileX,
						tileY,
						onFinish,
						new FinishListener() {
							@Override public void onFinish() {
								if (dir != null) {
									MGlobal.getHero().setFacing(dir);
								}
							}
				});
			}

			@Override protected boolean shouldFinish() {
				return super.shouldFinish() && teleportFinished;
			}
			
		});

		return LuaValue.NIL;
	}
}
