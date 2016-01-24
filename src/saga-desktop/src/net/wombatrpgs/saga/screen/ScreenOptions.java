/**
 *  ScreenOptions.java
 *  Created on Jan 23, 2016 7:34:51 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.screen;

import net.wombatrpgs.mgne.core.Constants;
import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.ui.Nineslice;
import net.wombatrpgs.mgne.ui.text.FontHolder;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.mgneschema.io.data.InputButton;
import net.wombatrpgs.mgneschema.io.data.InputCommand;
import net.wombatrpgs.saga.core.SConstants;

import com.badlogic.gdx.graphics.g2d.BitmapFont.HAlignment;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;

/**
 * Select text speed and controls and junk. I'm feeling really lazy at the end
 * of this project so most of the indices and stuff are hard coded. Sorry.
 */
public class ScreenOptions extends SagaScreen {
	
	protected static final int INFO_HEIGHT = 24;
	protected static final int FULL_WIDTH = 200;
	protected static final int FULL_HEIGHT = 180;
	protected static final int TEXT_HEIGHT = 12;
	protected static final int COLUMN_WIDTH = 96;
	
	protected Nineslice backer, infoBacker;
	protected TextFormat optionsFormat, infoFormat;
	
	protected String info;
	protected int selection;
	
	/**
	 * Default constructor.
	 */
	public ScreenOptions() {
		info = "Select an option.";
		
		backer = new Nineslice(FULL_WIDTH, FULL_HEIGHT - INFO_HEIGHT);
		infoBacker = new Nineslice(FULL_WIDTH, INFO_HEIGHT);
		assets.add(backer);
		assets.add(infoBacker);
		
		infoFormat = new TextFormat();
		infoFormat.x = getWidth()/2 - FULL_WIDTH/2 + 9;
		infoFormat.y = getHeight()/2 - FULL_HEIGHT/2 + FULL_HEIGHT - INFO_HEIGHT - infoBacker.getBorderHeight() + 16;
		infoFormat.align = HAlignment.LEFT;
		infoFormat.width = FULL_WIDTH - 32;
		infoFormat.height = TEXT_HEIGHT;
		
		optionsFormat = new TextFormat();
		optionsFormat.x = getWidth()/2 - FULL_WIDTH/2 + 22;
		optionsFormat.y = getHeight()/2 - FULL_HEIGHT/2 + FULL_HEIGHT - INFO_HEIGHT - infoBacker.getBorderHeight() - 10;
		optionsFormat.align = HAlignment.LEFT;
		optionsFormat.width = getWidth();
		optionsFormat.height = TEXT_HEIGHT;
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#render
	 * (com.badlogic.gdx.graphics.g2d.SpriteBatch)
	 */
	@Override
	public void render(SpriteBatch batch) {
		super.render(batch);
		backer.renderAt(batch, getWidth()/2 - FULL_WIDTH/2,
				getHeight()/2 - FULL_HEIGHT/2);
		infoBacker.renderAt(batch, getWidth()/2 - FULL_WIDTH/2,
				getHeight()/2 - FULL_HEIGHT/2 + FULL_HEIGHT - INFO_HEIGHT - infoBacker.getBorderHeight());
		
		FontHolder font = MGlobal.ui.getFont();
		font.draw(batch, infoFormat, info, 0);
		
		// welp
		for (int i = 0; i < 10; i += 1) {
			String option;
			String value;
			switch (i) {
			case 0:
				option = "TEXT SPEED";
				value = speedToString(Integer.valueOf(MGlobal.args.get(SConstants.ARG_TEXT_SPEED)));
				break;
			case 1:
				option = "SCREEN MODE";
				value = Boolean.valueOf(MGlobal.args.get(Constants.ARG_FULLSCREEN)) ? "FULL" : "WINDOW";
				break;
			case 2:
				option = "A BUTTON";
				value = controlToString(InputButton.BUTTON_A);
				break;
			case 3:
				option = "B BUTTON";
				value = controlToString(InputButton.BUTTON_B);
				break;
			case 4:
				option = "START";
				value = controlToString(InputButton.BUTTON_START);
				break;
			case 5:
				option = "SELECT";
				value = controlToString(InputButton.BUTTON_SELECT);
				break;
			case 6:
				option = "LEFT";
				value = controlToString(InputButton.LEFT);
				break;
			case 7:
				option = "UP";
				value = controlToString(InputButton.UP);
				break;
			case 8:
				option = "RIGHT";
				value = controlToString(InputButton.RIGHT);
				break;
			case 9:
				option = "DOWN";
				value = controlToString(InputButton.DOWN);
				break;
			default:
				option = "";
				value = "";
				MGlobal.reporter.err("OOB at screen options:" + i);
			}
			font.draw(batch, optionsFormat, option, -i * TEXT_HEIGHT);
			font.draw(batch, optionsFormat, value, COLUMN_WIDTH, -i * TEXT_HEIGHT);
		}
	}
	
	/**
	 * @see net.wombatrpgs.saga.screen.SagaScreen#onCommand
	 * (net.wombatrpgs.mgneschema.io.data.InputCommand)
	 */
	@Override
	public boolean onCommand(InputCommand command) {
		if (super.onCommand(command)) return true;
		switch (command) {
		case MOVE_UP:		moveCursor(-1);		return true;
		case MOVE_DOWN:		moveCursor(1);		return true;
		case UI_CONFIRM:	confirm();			return true;
		case UI_CANCEL:		cancel();			return true;
		case UI_FINISH:		cancel();			return true;
		default:								return true;
		}
	}

	/**
	 * Converts the save text speed int to something human readable.
	 * @param	speed			The text speed saved
	 * @return					A human readable string representing it
	 */
	protected String speedToString(int speed) {
		switch (speed) {
		case 0:		return "SLOW";
		case 1:		return "NORMAL";
		case 2:		return "FAST";
		case 3:		return "FASTEST";
		default:	return "?";
		}
	}
	
	/**
	 * Provides the human readable description of the control mapped to the
	 * given input button.
	 * @param	button			The button to get description for
	 * @return					The key used for that button
	 */
	protected String controlToString(InputButton button) {
		// TODO
		return "?";
	}
	
	/**
	 * Changes the cursor position.
	 * @param	delta			-1 or 1, the direction to move in
	 */
	protected void moveCursor(int delta) {
		selection += delta;
		if (selection >= 10) selection = 0;
		if (selection < 0) selection = 9;
	}
	
	/**
	 * Cancels the menu.
	 */
	protected void cancel() {
		SagaScreen title = new ScreenTitle();
		MGlobal.assets.loadAsset(title, "back to title");
		title.transitonOn(TransitionType.BLACK, null);
	}
	
	/**
	 * Called when user wants to change an option.
	 */
	protected void confirm() {
		
	}

}
