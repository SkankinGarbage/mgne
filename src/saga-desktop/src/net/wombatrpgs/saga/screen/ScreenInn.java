/**
 *  InnScreen.java
 *  Created on May 20, 2014 12:52:18 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.screen;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.utils.Align;

import net.wombatrpgs.mgne.core.MAssets;
import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.FinishListener;
import net.wombatrpgs.mgne.io.CommandListener;
import net.wombatrpgs.mgne.io.command.CMapMenu;
import net.wombatrpgs.mgne.ui.Nineslice;
import net.wombatrpgs.mgne.ui.Option;
import net.wombatrpgs.mgne.ui.OptionSelector;
import net.wombatrpgs.mgne.ui.text.FontHolder;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.mgneschema.io.data.InputCommand;
import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.chara.Chara;
import net.wombatrpgs.saga.ui.CharaSelector;
import net.wombatrpgs.sagaschema.rpg.stats.Stat;

/**
 * You heal using this thing.
 */
public class ScreenInn extends ScreenShop {
	
	protected static final int INFO_HEIGHT = 40;
	protected static final int INFO_MARGINS = 10;
	protected static final int INSERTS_PAD_Y = 4;
	
	protected static final int GLOBAL_Y = 82;
	
	protected CharaSelector inserts;
	protected Nineslice infoBG;
	protected TextFormat format;
	
	protected String payString;
	protected int payAmount;
	
	protected int insertsX, insertsY;
	
	/**
	 * Creates a new screen with the inn dialog.
	 */
	public ScreenInn() {
		menu = new OptionSelector(
				new Option("Pay") {
					@Override public boolean onSelect() { return onPay(); }
				},
				new Option("Leave") {
					@Override public boolean onSelect() { return onLeave(); }
				});
		menu.setCancel(new FinishListener() {
				@Override public void onFinish() {
					onLeave();
				}
		});
		assets.add(menu);
		
		infoBG = new Nineslice();
		assets.add(infoBG);
		
		inserts = new CharaSelector(true, false);
		inserts.setPadY(INSERTS_PAD_Y);
		addUChild(inserts);
		assets.add(inserts);
		
		FontHolder font = MGlobal.ui.getFont();
		insertsX = MGlobal.window.getViewportWidth()/2 - inserts.getWidth()/2;
		insertsY = GLOBAL_Y;

		payAmount = calcPayAmount();
		payString = "It will cost " + payAmount + " GP to stay here.";
		
		format = new TextFormat();
		format.align = Align.left;
		format.width = inserts.getWidth() - INFO_MARGINS / 2 - 10;
		format.height = INFO_HEIGHT;
		format.x = insertsX + INFO_MARGINS;
		format.y = (int) (insertsY + inserts.getHeight() + INFO_MARGINS +
				font.getLineHeight()*2  - inserts.getBorderHeight()) + 5;
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onFocusGained()
	 */
	@Override
	public void onFocusGained() {
		super.onFocusGained();
		menu.showAt(0, 0);
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#postProcessing
	 * (net.wombatrpgs.mgne.core.MAssets, int)
	 */
	@Override
	public void postProcessing(MAssets manager, int pass) {
		super.postProcessing(manager, pass);
		if (pass == 0) {
			inserts.setX(insertsX);
			inserts.setY(insertsY);
			infoBG.resizeTo(inserts.getWidth(), INFO_HEIGHT);
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#render
	 * (com.badlogic.gdx.graphics.g2d.SpriteBatch)
	 */
	@Override
	public void render(SpriteBatch batch) {
		super.render(batch);
		inserts.render(batch);
		infoBG.renderAt(batch, insertsX, insertsY + inserts.getHeight() - inserts.getBorderHeight());
		FontHolder font = MGlobal.ui.getFont();
		font.draw(batch, format, payString, 0);
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#dispose()
	 */
	@Override
	public void dispose() {
		super.dispose();
		infoBG.dispose();
	}
	
	/**
	 * Called when the user elects to pay the inn fee.
	 * @return					False to keep the menu open
	 */
	protected boolean onPay() {
		if (SGlobal.heroes.getGP() >= payAmount) {
			SGlobal.heroes.addGP(-1 * payAmount);
			SGlobal.heroes.innlikeHeal();
			payString = "HP restored.";
			inserts.refresh();
			MGlobal.audio.playSFX(SConstants.SFX_INN);
			menu.unfocus();
			pushCommandContext(new CMapMenu());
			pushCommandListener(new CommandListener() {
				@Override public boolean onCommand(InputCommand command) {
					if (command == InputCommand.UI_CONFIRM) {
						onLeave();
						return true;
					}
					return false;
				}
			});
		} else {
			payString = "Not enough GP.";
		}
		return false;
	}
	
	/**
	 * Calculates the fee the party needs to pay to stay at the inn.
	 * @return					The amount to pay, in GP
	 */
	protected int calcPayAmount() {
		int fee = 0;
		for (Chara member : SGlobal.heroes.getAll()) {
			fee += member.get(Stat.MHP) - member.get(Stat.HP);
		}
		return fee;
	}

}
