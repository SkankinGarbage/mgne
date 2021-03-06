/**
 *  ScenPathTo.java
 *  Created on Sep 1, 2014 3:34:45 PM for project mgne
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.scenes.commands;

import java.util.List;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.maps.events.MapEvent;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.mgne.util.AStarPathfinder;
import net.wombatrpgs.mgneschema.maps.data.OrthoDir;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.Varargs;
import org.luaj.vm2.lib.VarArgFunction;

/**
 * Oh god, A* pathfinding.
 * Usage: {@code path(<event> or <eventname>, <tilex>, <tiley>, [wait])}
 */
public class ScenePathToTile extends VarArgFunction {
	
	/**
	 * @see org.luaj.vm2.lib.VarArgFunction#invoke(org.luaj.vm2.Varargs)
	 */
	@Override
	public Varargs invoke(final Varargs args) {
		SceneLib.addFunction(new SceneCommand() {
			
			LuaValue eventArg = args.arg(1);
			int tileX = args.arg(2).checkint();
			int tileY = args.arg(3).checkint();
			boolean wait = args.narg() >= 4 ? args.checkboolean(4) : true;

			@Override protected void internalRun() {
				MapEvent event = argToEvent(eventArg);
				AStarPathfinder pather = new AStarPathfinder(
						MGlobal.levelManager.getActive(),
						event.getTileX(),
						event.getTileY(),
						tileX, MGlobal.levelManager.getActive().getHeight() - tileY - 1);
				List<OrthoDir> path = pather.getOrthoPath();
				if (path != null) {
					event.followPath(path);
				}
			}
			
			@Override protected boolean shouldFinish() {
				if (!super.shouldFinish()) return false;
				if (wait) {
					return !argToLua(eventArg).get("isTracking").call().checkboolean();
				} else {
					return true;
				}
			}
			
		});
		return LuaValue.NIL;
	}
	
}