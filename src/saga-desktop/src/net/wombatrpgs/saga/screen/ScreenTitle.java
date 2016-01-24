/**
 *  ScreenTitle.java
 *  Created on Jul 21, 2014 1:50:47 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.screen;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g2d.BitmapFont.HAlignment;

import net.wombatrpgs.mgne.core.Constants;
import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.FinishListener;
import net.wombatrpgs.mgne.io.audio.BackgroundMusic;
import net.wombatrpgs.mgne.io.command.CMapMenu;
import net.wombatrpgs.mgne.ui.Graphic;
import net.wombatrpgs.mgne.ui.text.FontHolder;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.mgneschema.io.data.InputCommand;
import net.wombatrpgs.mgneschema.settings.IntroSettingsMDO;
import net.wombatrpgs.saga.core.MemoryIndex;
import net.wombatrpgs.saga.core.SConstants;

/**
 * As might be obvious, this is the title screen.
 */
public class ScreenTitle extends SagaScreen {
	
	protected static final int TEXT_Y = 32;
	protected static final int TEXT_WIDTH = 120;
	protected static final int TEXT_SPACING = 96;
	
	protected static final String STRING_BEGIN = "START";
	protected static final String STRING_CONTINUE = "CONTINUE";
	protected static final String STRING_OPTIONS = "OPTIONS";
	protected static final String BGM_NAME = "ffl1_title";
	
	protected IntroSettingsMDO intro;
	protected Graphic bg;
	protected TextFormat textFormat;
	protected BackgroundMusic bgm;
	protected int selection;
	protected boolean transitioning;
	
	/**
	 * Creates the title screen!
	 */
	public ScreenTitle() {
		intro = MGlobal.data.getEntryFor(Constants.KEY_INTRO, IntroSettingsMDO.class);
		
		bg = new Graphic(intro.titleBG);
		assets.add(bg);
		
		textFormat = new TextFormat();
		textFormat.align = HAlignment.CENTER;
		textFormat.height = 32;
		textFormat.width = TEXT_WIDTH;
		textFormat.x = getWidth()/2 - TEXT_WIDTH/2;
		textFormat.y = TEXT_Y;
		
		bgm = MGlobal.audio.generateMusicForKey(BGM_NAME);
		assets.add(bgm);
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onFocusGained()
	 */
	@Override
	public void onFocusGained() {
		super.onFocusGained();
		pushCommandContext(new CMapMenu());
		MGlobal.audio.playBGM(bgm);
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#render(com.badlogic.gdx.graphics.g2d.SpriteBatch)
	 */
	@Override
	public void render(SpriteBatch batch) {
		super.render(batch);
		bg.renderAt(batch, 0, 0);
		
		FontHolder font = MGlobal.ui.getFont();
		font.draw(batch, textFormat, STRING_BEGIN, -1 * TEXT_SPACING, 0);
		font.draw(batch, textFormat, STRING_CONTINUE, 0, 0);
		font.draw(batch, textFormat, STRING_OPTIONS, TEXT_SPACING, 0);
		
		Graphic cursor = MGlobal.ui.getCursor();
		int offX = cursor.getWidth() * -1;
		int offY = cursor.getHeight() * -1;
		float textWidth;
		switch (selection) {
			case 0:
				textWidth = font.getWidth(STRING_BEGIN);
				cursor.renderAt(batch, getWidth()/2 - textWidth/2 - TEXT_SPACING + offX, TEXT_Y + offY);
				break;
			case 1:
				textWidth = font.getWidth(STRING_CONTINUE);
				cursor.renderAt(batch, getWidth()/2 - textWidth/2 + offX, TEXT_Y + offY);
				break;
			case 2:
				textWidth = font.getWidth(STRING_OPTIONS);
				cursor.renderAt(batch, getWidth()/2 - textWidth/2 + TEXT_SPACING + offX, TEXT_Y + offY);
				break;
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onCommand
	 * (net.wombatrpgs.mgneschema.io.data.InputCommand)
	 */
	@Override
	public boolean onCommand(InputCommand command) {
		if (super.onCommand(command)) return true;
		if (transitioning) return true;
		switch (command) {
			case MOVE_LEFT:		return moveCursor(-1);
			case MOVE_RIGHT:	return moveCursor(1);
			case UI_CONFIRM:	return confirm();
			case UI_FINISH:		return confirm();
			case UI_CANCEL:		Gdx.app.exit(); return true;
			default:			return false;
		}
	}
		
	/**
	 * Moves the selection cursor one way or another.
	 * @param	delta			The delta to move cursor by
	 * @return					True to indicate command was processed
	 */
	public boolean moveCursor(int delta) {
		selection += delta;
		if (selection < 0) selection = 2;
		if (selection > 2) selection = 0;
		return true;
	}

	/**
	 * Called when the user confirms their selection on the menu.
	 * @return					True to indicate command was processed
	 */
	public boolean confirm() {
		switch (selection) {
			case 0:
				// start
				transitioning = true;
				SagaScreen textIntro = new ScreenTextIntro();
				MGlobal.assets.loadAsset(textIntro, "text intro");
				textIntro.transitonOn(TransitionType.BLACK, new FinishListener() {
					@Override public void onFinish() {
						dispose();
					}
				});
				return true;
			case 1:
				// load
				if (MemoryIndex.loadIndex().existingSavesCount() > 0) {
					transitioning = true;
					SagaScreen screenSave = new ScreenSaves(false);
					MGlobal.assets.loadAsset(screenSave, "save intro");
					screenSave.transitonOn(TransitionType.BLACK, new FinishListener() {
						@Override public void onFinish() {
							dispose();
						}
					});
				} else {
					MGlobal.audio.playSFX(SConstants.SFX_FAIL);
				}
				return true;
			case 2:
				// options
				SagaScreen optionsScreen = new ScreenOptions();
				MGlobal.assets.loadAsset(optionsScreen, "options screen");
				optionsScreen.transitonOn(TransitionType.BLACK, new FinishListener() {
					@Override public void onFinish() {
						dispose();
					}
				});
				return true;
			default:
				MGlobal.reporter.err("Unknown title index: " + selection);
				return true;
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#dispose()
	 */
	@Override
	public void dispose() {
		super.dispose();
		bgm.dispose();
	}
	
}
