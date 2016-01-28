/**
 *  ScreenOptions.java
 *  Created on Jan 23, 2016 7:34:51 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.screen;

import java.util.ArrayList;

import net.wombatrpgs.mgne.core.Constants;
import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.io.CommandMap;
import net.wombatrpgs.mgne.io.InputListener;
import net.wombatrpgs.mgne.io.command.CMapMenu;
import net.wombatrpgs.mgne.maps.objects.TimerListener;
import net.wombatrpgs.mgne.maps.objects.TimerObject;
import net.wombatrpgs.mgne.screen.Screen;
import net.wombatrpgs.mgne.ui.Nineslice;
import net.wombatrpgs.mgne.ui.text.FontHolder;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.mgneschema.io.data.GdxKey;
import net.wombatrpgs.mgneschema.io.data.InputButton;
import net.wombatrpgs.mgneschema.io.data.InputCommand;
import net.wombatrpgs.saga.core.SConstants;

import com.badlogic.gdx.utils.Align;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;

/**
 * Select text speed and controls and junk. I'm feeling really lazy at the end
 * of this project so most of the indices and stuff are hard coded. Sorry.
 */
public class ScreenOptions extends SagaScreen {
	
	protected static final int INFO_HEIGHT = 24;
	protected static final int FULL_WIDTH = 240;
	protected static final int FULL_HEIGHT = 180;
	protected static final int TEXT_HEIGHT = 12;
	protected static final int COLUMN_WIDTH = 100;
	
	protected static final float KEY_TIMEOUT_SECONDS = 5.0f;
	
	protected Nineslice backer, infoBacker;
	protected TextFormat optionsFormat, infoFormat;
	protected CommandMap menuContext;
	
	protected String info;
	protected InputButton awaitingButton;
	protected InputListener inputListener;
	protected float elapsed;
	protected int selection;
	
	/**
	 * Default constructor.
	 */
	public ScreenOptions() {
		updateInfo();
		
		menuContext = new CMapMenu();
		
		inputListener = new InputListener() {
			@Override public void onKeyUp(int keycode) { }
			@Override public void onKeyDown(int keycode) {
				changeButtonToKeycode(keycode);
			}
		};
		
		backer = new Nineslice(FULL_WIDTH, FULL_HEIGHT - INFO_HEIGHT);
		infoBacker = new Nineslice(FULL_WIDTH, INFO_HEIGHT);
		assets.add(backer);
		assets.add(infoBacker);
		
		infoFormat = new TextFormat();
		infoFormat.x = getWidth()/2 - FULL_WIDTH/2 + 9;
		infoFormat.y = getHeight()/2 - FULL_HEIGHT/2 + FULL_HEIGHT - 
				INFO_HEIGHT - infoBacker.getBorderHeight() + 16;
		infoFormat.align = Align.left;
		infoFormat.width = FULL_WIDTH - 32;
		infoFormat.height = TEXT_HEIGHT;
		
		optionsFormat = new TextFormat();
		optionsFormat.x = getWidth()/2 - FULL_WIDTH/2 + 23;
		optionsFormat.y = getHeight()/2 - FULL_HEIGHT/2 + FULL_HEIGHT - 
				INFO_HEIGHT - infoBacker.getBorderHeight() - 10;
		optionsFormat.align = Align.left;
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
			default:
				option = buttonForSlot(i).getDisplayName();
				value = controlToString(buttonForSlot(i));
				break;
			}
			font.draw(batch, optionsFormat, option, -i * TEXT_HEIGHT);
			font.draw(batch, optionsFormat, value, COLUMN_WIDTH, -i * TEXT_HEIGHT);
		}
		
		MGlobal.ui.getCursor().renderAt(batch, optionsFormat.x - 17,
				optionsFormat.y - selection * TEXT_HEIGHT - 16);
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onFocusGained()
	 */
	@Override
	public void onFocusGained() {
		super.onFocusGained();
		pushCommandContext(menuContext);
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onFocusLost()
	 */
	@Override
	public void onFocusLost() {
		super.onFocusLost();
		removeCommandContext(menuContext);
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
	 * @see net.wombatrpgs.saga.screen.SagaScreen#update(float)
	 */
	@Override
	public void update(float delta) {
		super.update(delta);
		if (awaitingButton != null) {
			elapsed += delta;
			if (elapsed > KEY_TIMEOUT_SECONDS) {
				cancelButton();
			}
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
		if (MGlobal.args.get(MGlobal.keymap.configForButton(button)) != null) {
			return MGlobal.args.get(MGlobal.keymap.configForButton(button));
		} else {
			switch (button) {
			case BUTTON_A:			return "z/space/enter";
			case BUTTON_B:			return "x/backspace";
			case BUTTON_START:		return "c/esc";
			case BUTTON_SELECT:		return "v/tab";
			case DOWN:				// fall through
			case UP:				// fall through
			case RIGHT:				// fall through
			case LEFT:				return "arrow key";
			default:				return "?";
			}
		}
	}
	
	/**
	 * Changes the cursor position.
	 * @param	delta			-1 or 1, the direction to move in
	 */
	protected void moveCursor(int delta) {
		selection += delta;
		if (selection >= 10) selection = 0;
		if (selection < 0) selection = 9;
		updateInfo();
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
		switch (selection) {
		case 0:			cycleTextSpeed();							break;
		case 1:			toggleFullscreen();							break;
		default:		changeButton(buttonForSlot(selection));		break;
		}
	}
	
	/**
	 * Increment text speed on the counter.
	 */
	protected void cycleTextSpeed() {
		Integer newValue = Integer.valueOf(MGlobal.args.get(SConstants.ARG_TEXT_SPEED)) + 1;
		if (newValue > 3) newValue = 0;
		MGlobal.args.put(SConstants.ARG_TEXT_SPEED, newValue.toString());
		saveConfigs();
	}
	
	/**
	 * Toggles full screen mode.
	 */
	protected void toggleFullscreen() {
		Boolean newValue = !Boolean.valueOf(MGlobal.args.get(Constants.ARG_FULLSCREEN));
		MGlobal.args.put(Constants.ARG_FULLSCREEN, newValue.toString());
		Screen.setFullscreen(newValue);
		saveConfigs();
	}
	
	/**
	 * Prompt for a new binding for the provided button.
	 * @param	button			The button to get a new binding for
	 */
	protected void changeButton(InputButton button) {
		info = "Press new button, or wait";
		awaitingButton = button;
		elapsed = 0;
		MGlobal.keymap.registerInputListener(inputListener);
	}
	
	/**
	 * Binds the awaiting button to the pressed keycode.
	 * @param	keycode			The code of the pressed key
	 */
	protected void changeButtonToKeycode(int keycode) {
		String result = null;
		for (GdxKey key : GdxKey.values()) {
			if (key.keycode == keycode) {
				result = key.name();
				break;
			}
		}
		if (result == null) {
			result = String.valueOf(keycode);
		}
		MGlobal.args.put(MGlobal.keymap.configForButton(awaitingButton), result);
		info = "Set to " + result;
		finishButtonChange();
	}
	
	/**
	 * Cancel listening for an input button
	 */
	protected void cancelButton() {
		info = "Reset to default.";
		MGlobal.args.remove(MGlobal.keymap.configForButton(awaitingButton));
		finishButtonChange();
	}
	
	/**
	 * Finishes the button config process by saving.
	 */
	protected void finishButtonChange() {
		new TimerObject(0.0f, this, new TimerListener() {
			@Override public void onTimerZero(TimerObject source) {
				MGlobal.keymap.unregisterInputListener(inputListener);
			}
		});
		awaitingButton = null;
		elapsed = 0;
		saveConfigs();
	}
	
	/**
	 * Writes the configs to disk.
	 */
	protected void saveConfigs() {
		ArrayList<String> configs = new ArrayList<String>();
		configs.add(Constants.ARG_FULLSCREEN);
		configs.add(SConstants.ARG_TEXT_SPEED);
		for (int i = 2; i < 10; i += 1) {
			configs.add(MGlobal.keymap.configForButton(buttonForSlot(i)));
		}
		MGlobal.files.writeConfigs(configs);
	}
	
	/**
	 * Sets the top information string according to selection.
	 */
	protected void updateInfo() {
		switch (selection) {
		case 0:		info = "Message autotype speed";		break;
		case 1:		info = "Full screen or window";			break;
		default:	info = "Custom button config";			break;
		}
	}
	
	/**
	 * Given an option slot, returns the associated button.
	 * @param	slot			The slot to check, should be 2-9
	 * @return					The button associated with that slot
	 */
	protected InputButton buttonForSlot(int slot) {
		switch (slot) {
		case 2:		return InputButton.BUTTON_A;
		case 3:		return InputButton.BUTTON_B;
		case 4:		return InputButton.BUTTON_START;
		case 5:		return InputButton.BUTTON_SELECT;
		case 6:		return InputButton.LEFT;
		case 7:		return InputButton.UP;
		case 8:		return InputButton.RIGHT;
		case 9:		return InputButton.DOWN;
		default:	MGlobal.reporter.err("Slot OOB: " + slot);		return null;
		}
	}
}
