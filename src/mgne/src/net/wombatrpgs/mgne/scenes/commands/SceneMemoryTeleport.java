/**
 *  SceneMemoryTeleport.java
 *  Created on May 19, 2015 11:16:32 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.scenes.commands;

import java.util.HashMap;
import java.util.Map;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.maps.Level;
import net.wombatrpgs.mgne.maps.events.MapEvent;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.Varargs;
import org.luaj.vm2.lib.VarArgFunction;

/**
 * A teleport, but goes to an intermediate map first. Takes two map names. The
 * first is an intermediary map, while the second is the actual final map. For
 * keys, 'ENTRANCE' is to be used by the doors leading into the memory room, and
 * 'EXIT' is to be used by doors leading out from the memory room. 'A' and 'B'
 * are arbitrary and should each have an associated entrance and exit. Both of
 * the entrances need an event named 'exit' that marks where reverse teleports
 * land, and the memory room itself needs events named 'target_a' and 'target_b'
 * that mark the landing points of the incoming doors.
 * Usage: {@code memTele(<intermap.tmx>, <final.tmx>, <entrance/exit>, <a/b>)}
 */
public class SceneMemoryTeleport extends VarArgFunction {
	
	private static final String NAME_TARGET_A = "target_a";
	private static final String NAME_TARGET_B = "target_b";
	
	// global state is unfortunately the path of least resistance here
	// this has to be shared by separate mark teleports
	// public so it can be saved in a dumb way
	public static Map<DoorSet, String> mapSet = new HashMap<DoorSet, String>();
	
	public enum DoorType { ENTRANCE, EXIT };
	public enum DoorSet { A, B };

	/**
	 * @see org.luaj.vm2.lib.VarArgFunction#invoke(org.luaj.vm2.Varargs)
	 */
	@Override
	public Varargs invoke(final Varargs args) {
		SceneLib.addFunction(new SceneCommand() {
			
			String interMapKey;
			String finalMapKey;
			DoorType type;
			DoorSet set;
			
			/* Initializer */ {
				interMapKey = args.arg(1).checkjstring();
				finalMapKey = args.arg(2).checkjstring();
				type = DoorType.valueOf(args.arg(3).checkjstring());
				set = DoorSet.valueOf(args.arg(4).checkjstring());
			}

			@Override protected void internalRun() {
				if (type == DoorType.ENTRANCE) {
					mapSet.put(set, MGlobal.getHero().getParent().getKeyName());
					mapSet.put(set == DoorSet.A ? DoorSet.B : DoorSet.A, finalMapKey);
					teleportToEvent(interMapKey, targetNameFor(set));
				} else {
					String key = mapSet.get(set);
					teleportToEvent(key, targetNameFor(set));
				}
			}
			
			private void teleportToEvent(String mapKey, String eventName) {
				Level map = MGlobal.levelManager.getLevel(mapKey);
				MapEvent target = map.getEventByName(eventName);
				int tileX = target.getTileX();
				int tileY = map.getHeight() - target.getTileY() - 1;
				MGlobal.levelManager.getTele().teleport(mapKey, tileX, tileY);
			}
			
			private String targetNameFor(DoorSet set) {
				return (set == DoorSet.A) ? NAME_TARGET_A : NAME_TARGET_B;
			}
			
		});
		return LuaValue.NIL;
	}
}
