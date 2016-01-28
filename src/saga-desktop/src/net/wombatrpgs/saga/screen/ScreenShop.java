/**
 *  ScreenShop.java
 *  Created on Jan 10, 2016 3:01:03 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.screen;

import com.badlogic.gdx.utils.Align;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.ui.Nineslice;
import net.wombatrpgs.mgne.ui.OptionSelector;
import net.wombatrpgs.mgne.ui.text.FontHolder;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.saga.core.SGlobal;

/**
 * Superclass for the item and meat shops.
 */
public class ScreenShop extends SagaScreen {
	
	protected static final int GP_WIDTH = 90;
	protected static final int GP_HEIGHT = 24;
	protected static final int GP_MARGINS = 8;
	
	protected OptionSelector menu;
	protected TextFormat gpFormat;
	protected Nineslice gpBG;
	protected float gpX, gpY;
	protected boolean done;
	
	/**
	 * Creates the default shop stuff.
	 */
	public ScreenShop() {
		gpBG = new Nineslice(GP_WIDTH, GP_HEIGHT);
		assets.add(gpBG);
		
		gpX = getWidth() - GP_WIDTH;
		gpY = 0;
		
		FontHolder font = MGlobal.ui.getFont();
		gpFormat = new TextFormat();
		gpFormat.align = Align.right;
		gpFormat.width = GP_WIDTH - 2 * GP_MARGINS;
		gpFormat.height = GP_HEIGHT;
		gpFormat.x = getWidth() - GP_WIDTH + GP_MARGINS;
		gpFormat.y = (int) (GP_MARGINS + font.getLineHeight());
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#render
	 * (com.badlogic.gdx.graphics.g2d.SpriteBatch)
	 */
	@Override
	public void render(SpriteBatch batch) {
		super.render(batch);
		gpBG.renderAt(batch, gpX, gpY);
		String gpString = SGlobal.heroes.getGP() + " GP";
		FontHolder font = MGlobal.ui.getFont();
		font.draw(batch, gpFormat, gpString, 0);
	}
	
	/**
	 * Checks if this inn screen is done and should be disposed by the calling
	 * context, usually a lua function.
	 * @return					True to dispose this screen, false if not yet
	 */
	public boolean isDone() {
		return done;
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#dispose()
	 */
	@Override
	public void dispose() {
		super.dispose();
		gpBG.dispose();
		menu.dispose();
	}

	/**
	 * Called when the user elects to leave the shop.
	 * @return					True to close the menu
	 */
	protected boolean onLeave() {
		menu.close();
		MGlobal.screens.pop();
		done = true;
		return true;
	}
	

}
