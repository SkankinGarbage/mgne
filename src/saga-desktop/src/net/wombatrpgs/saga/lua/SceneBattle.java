/**
 *  SceneBattle.java
 *  Created on Apr 19, 2014 1:32:17 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.lua;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.scenes.SceneCommand;
import net.wombatrpgs.mgne.scenes.SceneLib;
import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.rpg.battle.Battle;
import net.wombatrpgs.saga.rpg.chara.EnemyParty;
import net.wombatrpgs.sagaschema.rpg.chara.PartyMDO;
import net.wombatrpgs.sagaschema.rpg.encounter.EncounterMDO;

import org.luaj.vm2.LuaValue;
import org.luaj.vm2.Varargs;
import org.luaj.vm2.lib.VarArgFunction;

/**
 * Starts a battle against the party specificied by the argument data key.
 * Usage: {@code battle(<mdo>, [fleeable], [bgm])}
 */
public class SceneBattle extends VarArgFunction {

	/**
	 * @see org.luaj.vm2.lib.VarArgFunction#invoke(org.luaj.vm2.Varargs)
	 */
	@Override
	public Varargs invoke(final Varargs args) {
		SceneLib.addFunction(new SceneCommand() {
			
			Battle battle;
			
			String mdoKey;
			boolean fleeable;
			String bgmKey;
			
			/* Initializer */ {
				mdoKey = args.arg(1).checkjstring();
				fleeable = args.narg() >= 2 ? args.checkboolean(2) : true;
				bgmKey = args.narg() >= 3 ? args.checkjstring(3) : null;
			}

			@Override protected void internalRun() {
				// battle stuff moved here, problems before with null heroes
				MGlobal.audio.playSFX(SConstants.SFX_BATTLE);
				PartyMDO partyMDO = MGlobal.data.getIfExists(mdoKey, PartyMDO.class);
				if (partyMDO != null) {
					battle = new Battle(partyMDO, true);
				} else {
					EncounterMDO encounterMDO = MGlobal.data.getEntryFor(mdoKey, EncounterMDO.class);
					EnemyParty party = new EnemyParty(encounterMDO);
					battle = new Battle(party, true);
				}
				battle.setFleeable(fleeable);
				battle.setBGM(bgmKey);
				MGlobal.assets.loadAsset(battle, "scene battle " + mdoKey);
				battle.start();
			}

			@Override protected void finish() {
				super.finish();
				if (battle.wasAborted()) {
					parent.abort();
				}
				battle.dispose();
			}

			@Override protected boolean shouldFinish() {
				return super.shouldFinish() && battle.isDone();
			}
			
		});
		return LuaValue.NIL;
	}

}
