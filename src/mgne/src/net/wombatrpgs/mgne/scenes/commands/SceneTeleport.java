/**
 *  SceneTeleport.java
 *  Created on Mar 31, 2014 11:19:55 AM for project mgne
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.scenes.commands;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.FinishListener;
import net.wombatrpgs.mgne.maps.Level;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.mgneschema.maps.data.OrthoDir;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.Varargs;
import org.luaj.vm2.lib.VarArgFunction;

/**
 * Teleports hero to some remote locale.
 * Usage: {@code teleport(<mapname>, <x>, <y>, [direction], [transitionEnabled])}
 */
public class SceneTeleport extends VarArgFunction {

	/**
	 * @see org.luaj.vm2.lib.VarArgFunction#invoke(org.luaj.vm2.Varargs)
	 */
	@Override
	public Varargs invoke(final Varargs args) {
		SceneLib.addFunction(new SceneCommand() {
			
			String mapName;
			OrthoDir dir;
			int tileX, tileY;
			boolean teleportFinished;
			boolean useTransition;
			
			/* Initializer */ {
				mapName = args.arg(1).checkjstring();
				tileX = args.arg(2).checkint();
				tileY = args.arg(3).checkint();
				dir = (args.narg() >= 4) ? OrthoDir.valueOf(args.arg(4).checkjstring()) : null;
				useTransition = (args.narg() >= 5) ? args.arg(5).checkboolean() : true;
				teleportFinished = false;
			}

			@Override protected void internalRun() {
				if (useTransition) {
					final Level map = MGlobal.levelManager.getLevel(mapName);
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
				} else {
					MGlobal.levelManager.getTele().teleportRaw(mapName,
							tileX,
							tileY);
					if (dir != null) {
						MGlobal.getHero().setFacing(dir);
					}
					Level map = MGlobal.levelManager.getActive();
					map.update(0);
					teleportFinished = true;
				}
			}

			@Override protected boolean shouldFinish() {
				return super.shouldFinish() && teleportFinished;
			}
			
		});

		return LuaValue.NIL;
	}

}
