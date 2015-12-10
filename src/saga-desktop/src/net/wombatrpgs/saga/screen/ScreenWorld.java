/**
 *  WorldScreen.java
 *  Created on Mar 28, 2014 5:14:00 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.screen;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.io.command.CMapGame;
import net.wombatrpgs.mgne.maps.Level;
import net.wombatrpgs.mgneschema.io.data.InputCommand;
import net.wombatrpgs.saga.core.SGlobal;

/**
 * Shows up when you wander the overworld.
 */
public class ScreenWorld extends SagaScreen {
	
	protected static final String KEY_BATTLE_BGM = "battle_bgm";
	protected static final String KEY_FLOOR = "floor";
	
	/**
	 * Constructs the SaGa world screen.
	 */
	public ScreenWorld() {
		pushCommandContext(new CMapGame());
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.instances.ScreenGame#onCommand
	 * (net.wombatrpgs.mgneschema.io.data.InputCommand)
	 */
	@Override
	public boolean onCommand(InputCommand command) {
		if (super.onCommand(command)) return true;
		switch (command) {
		case WORLD_PAUSE:
			ScreenPause menu = new ScreenPause();
			MGlobal.assets.loadAsset(menu, "main menu");
			MGlobal.screens.push(menu);
			return true;
		default:
			return MGlobal.getHero().onCommand(command);
		}
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#update(float)
	 */
	@Override
	public void update(float elapsed) {
		super.update(elapsed);
		if (MGlobal.levelManager.getActive() != null) {
			MGlobal.levelManager.getActive().update(elapsed);
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#render
	 * (com.badlogic.gdx.graphics.g2d.SpriteBatch)
	 */
	@Override
	public void render(SpriteBatch batch) {
		MGlobal.levelManager.getActive().render(batch);
		super.render(batch);
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onMapGainedFocus(net.wombatrpgs.mgne.maps.Level)
	 */
	@Override
	public void onMapGainedFocus(Level map) {
		super.onMapGainedFocus(map);
		if (map.getProperty(KEY_BATTLE_BGM) != null) {
			SGlobal.battleBGMKey = map.getProperty(KEY_BATTLE_BGM);
		}
	}

}
