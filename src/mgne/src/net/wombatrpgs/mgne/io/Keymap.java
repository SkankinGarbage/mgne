/**
 *  Keymap.java
 *  Created on Nov 19, 2012 2:31:14 PM for project rainfall-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.io;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import net.wombatrpgs.mgne.core.Constants;
import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.Updateable;
import net.wombatrpgs.mgne.graphics.interfaces.Disposable;
import net.wombatrpgs.mgne.io.InputEvent.EventType;
import net.wombatrpgs.mgneschema.io.KeymapMDO;
import net.wombatrpgs.mgneschema.io.data.GdxKey;
import net.wombatrpgs.mgneschema.io.data.InputButton;
import net.wombatrpgs.mgneschema.io.data.KeyButtonPairMDO;
import net.wombatrpgs.mgneschema.ui.InputSettingsMDO;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.InputProcessor;
import com.badlogic.gdx.controllers.Controller;
import com.badlogic.gdx.controllers.ControllerListener;
import com.badlogic.gdx.controllers.Controllers;
import com.badlogic.gdx.controllers.PovDirection;
import com.badlogic.gdx.math.Vector3;

/**
 * A map from physical keyboard keys to the meta-buttons that the game runs on.
 * 
 * As of 2013-01-31, constantly sends the down event as long as the key is
 * held down. This may lead to some weird issues, but it's much better than the
 * mandatory 1:1 mapping alternative.
 * 
 * Changed on 2014-01-21, contains a lot more functionality for key repeat.
 */
public class Keymap implements	InputProcessor,
								Updateable,
								Disposable,
								ControllerListener {
	
	protected static final float CONTROLLER_DEAD_ZONE = 0.5f;

	protected KeymapMDO mdo;
	protected Controller activeController;
	
	protected List<ButtonListener> buttonListeners;
	protected List<InputListener> inputListeners;
	protected List<InputEvent> queue;
	protected Map<InputButton, KeyState> states;
	protected Map<Integer, InputButton> keyToButton;
	protected Map<InputButton, List<Integer>> buttonToKey;
	protected Map<ControllerAxis, Float> axisZeroValues;
	
	/**
	 * Creates and intializes a new keymap.
	 * @param	mdo				The data to create from
	 */
	public Keymap(KeymapMDO mdo) {
		this.mdo = mdo;
		queue = new ArrayList<InputEvent>();
		buttonListeners = new ArrayList<ButtonListener>();
		inputListeners = new ArrayList<InputListener>();
		states = new HashMap<InputButton, KeyState>();
		keyToButton = new HashMap<Integer, InputButton>();
		buttonToKey = new HashMap<InputButton, List<Integer>>();
		axisZeroValues = new HashMap<Keymap.ControllerAxis, Float>();
		
		for (InputButton button : InputButton.values()) {
			buttonToKey.put(button, new ArrayList<Integer>());
		}
		for (KeyButtonPairMDO pairMDO : mdo.bindings) {
			keyToButton.put(pairMDO.keyCode.keycode, pairMDO.button);
			buttonToKey.get(pairMDO.button).add(pairMDO.keyCode.keycode);
		}
		
		Controllers.addListener(this);
		
		clearState();
	}
	
	/**
	 * Creates the default keymap by checking the database for the MDO as
	 * defined in Constants. Should be called from SGlobal.
	 * @return					The created keymap
	 */
	public static Keymap initDefaultKeymap() {
		InputSettingsMDO inputMDO = MGlobal.data.getEntryFor(
				Constants.KEY_INPUT, InputSettingsMDO.class);
		KeymapMDO keyMDO = MGlobal.data.getEntryFor(inputMDO.keymap, KeymapMDO.class);
		Keymap map = new Keymap(keyMDO);
		map.registerButtonListener(MGlobal.screens);
		Gdx.input.setInputProcessor(map);
		return map;
	}
	
	/**
	 * @see net.wombatrpgs.mgne.core.interfaces.Updateable#update(float)
	 */
	@Override
	public void update(float elapsed) {
		for (InputButton button : InputButton.values()) {
			if (states.get(button) == KeyState.DOWN) {
				if (!buttonDown(button)) {
					// this is to prevent loss-of-focus getting state unsync'd
					states.put(button, KeyState.UP);
					queue.add(new InputEvent(button, EventType.RELEASE));
				} else {
					queue.add(new InputEvent(button, EventType.HOLD));
				}
			}
		}
		while (queue.size() > 0) {
			InputEvent next = queue.get(0);
			queue.remove(0);
			signal(next);
		}
		queue.clear();
	}

	/**
	 * @see net.wombatrpgs.mgne.graphics.interfaces.Disposable#dispose()
	 */
	@Override
	public void dispose() {
		if (Gdx.input.getInputProcessor() == this) {
			Gdx.input.setInputProcessor(null);
		}
		if (MGlobal.keymap == this) {
			MGlobal.keymap = null;
		}
		Controllers.removeListener(this);
	}

	/**
	 * Registers a new object to listen for meta-button presses.
	 * @param 	listener		The listener to register
	 */
	public void registerButtonListener(ButtonListener listener) {
		buttonListeners.add(listener);
	}
	
	/**
	 * Unregisters an existing listener from meta-button presses.
	 * @param 	listener		The listener to unregister
	 */
	public void unregisterButtonListener(ButtonListener listener) {
		if (buttonListeners.contains(listener)) {
			buttonListeners.remove(listener);
		} else {
			MGlobal.reporter.warn("The listener " + listener + " is not " +
					"actually listening to " + this);
		}
	}
	
	/**
	 * Registers a new object to listen for raw button presses.
	 * @param	listener		The listener to register
	 */
	public void registerInputListener(InputListener listener) {
		inputListeners.add(listener);
	}
	
	/**
	 * Unregisters an existing listener from raw button presses.
	 * @param 	listener		The listener to unregister
	 */
	public void unregisterInputListener(InputListener listener) {
		if (inputListeners.contains(listener)) {
			inputListeners.remove(listener);
		} else {
			MGlobal.reporter.warn("The listener " + listener + " is not " +
					"actually listening to " + this);
		}
	}
	
	/**
	 * Gets the state of a specific input button. This should really only be
	 * called by the command map as part of a reverse-mapping for very specific
	 * polling situations.
	 * @param	button			The button to fetch state
	 * @return					The current state of that button's key
	 */
	public KeyState getButtonState(InputButton button) {
		return states.get(button);
	}
	
	/**
	 * Removes all listeners from a stale keymap and copies them to this one.
	 * Helpful for loading. Takes care of disposing the old keymap.
	 * @param	stale			The keymap that is to die
	 */
	public void absorbListeners(Keymap stale) {
		if (stale != this) {
			for (ButtonListener listener : stale.buttonListeners) {
				buttonListeners.add(listener);
			}
			if (Gdx.input.getInputProcessor() == stale) {
				Gdx.input.setInputProcessor(this);
			}
			stale.dispose();
		} else {
			MGlobal.reporter.warn("Trying to self-absorb keymap " + this);
		}
	}
	
	/**
	 * Clears the up/down state of all buttons.
	 */
	public void clearState() {
		for (InputButton button : InputButton.values()) {
			states.put(button, KeyState.UP);
		}
		queue.clear();
	}

	/**
	 * We'll handle the keybindings here.
	 * @see com.badlogic.gdx.InputProcessor#keyDown(int)
	 */
	@Override
	public boolean keyDown(int keycode) {
		InputButton button = keyToButton(keycode);
		for (InputListener listener : inputListeners) {
			listener.onKeyDown(keycode);
		}
		if (button == null) {
			return false;
		}
		if (states.get(button) == KeyState.UP) {
			queue.add(new InputEvent(button, EventType.PRESS));
			queue.add(new InputEvent(button, EventType.HOLD));
		}
		states.put(button, KeyState.DOWN);
		return true;
	}

	/**
	 * We'll handle the keybindings here.
	 * @see com.badlogic.gdx.InputProcessor#keyUp(int)
	 */
	@Override
	public boolean keyUp(int keycode) {
		// most of this is handled in update
		for (InputListener listener : inputListeners) {
			listener.onKeyUp(keycode);
		}
		return false;
	}

	/**
	 * Override if needed by the specific keymapping.
	 * @see com.badlogic.gdx.InputProcessor#keyTyped(char)
	 */
	@Override
	public boolean keyTyped(char character) {
		queue.add(new InputEvent(character));
		return true;
	}

	/**
	 * Override if needed by the specific keymapping.
	 * @see com.badlogic.gdx.InputProcessor#touchDown(int, int, int, int)
	 */
	@Override
	public boolean touchDown(int x, int y, int pointer, int button) {
		return false;
	}

	/**
	 * Override if needed by the specific keymapping.
	 * @see com.badlogic.gdx.InputProcessor#touchUp(int, int, int, int)
	 */
	@Override
	public boolean touchUp(int x, int y, int pointer, int button) {
		return false;
	}

	/**
	 * Override if needed by the specific keymapping.
	 * @see com.badlogic.gdx.InputProcessor#touchDragged(int, int, int)
	 */
	@Override
	public boolean touchDragged(int x, int y, int pointer) {
		return false;
	}

	/**
	 * Override if needed by the specific keymapping.
	 * @see com.badlogic.gdx.InputProcessor#scrolled(int)
	 */
	@Override
	public boolean scrolled(int amount) {
		return false;
	}
	
	/**
	 * Override if needed etc etc
	 * @see com.badlogic.gdx.InputProcessor#mouseMoved(int, int)
	 */
	@Override
	public boolean mouseMoved(int screenX, int screenY) {
		return false;
	}
	
	/**
	 * @see com.badlogic.gdx.controllers.ControllerListener#connected
	 * (com.badlogic.gdx.controllers.Controller)
	 */
	@Override
	public void connected(Controller controller) {
		// no op
	}

	/**
	 * @see com.badlogic.gdx.controllers.ControllerListener#disconnected
	 * (com.badlogic.gdx.controllers.Controller)
	 */
	@Override
	public void disconnected(Controller controller) {
		// no op
	}

	/**
	 * @see com.badlogic.gdx.controllers.ControllerListener#buttonDown
	 * (com.badlogic.gdx.controllers.Controller, int)
	 */
	@Override
	public boolean buttonDown(Controller controller, int buttonCode) {
		// treat this like we would any other key
		return keyDown(buttonCode);
	}

	/**
	 * @see com.badlogic.gdx.controllers.ControllerListener#buttonUp
	 * (com.badlogic.gdx.controllers.Controller, int)
	 */
	@Override
	public boolean buttonUp(Controller controller, int buttonCode) {
		// treat this like we would any other key
		return keyUp(buttonCode);
	}

	/**
	 * @see com.badlogic.gdx.controllers.ControllerListener#axisMoved
	 * (com.badlogic.gdx.controllers.Controller, int, float)
	 */
	@Override
	public boolean axisMoved(Controller controller, int axisCode, float value) {
		ControllerAxis controllerAxis = new ControllerAxis(controller, axisCode);
		Float zeroValue = axisZeroValues.get(controllerAxis);
		if (zeroValue == null) {
			axisZeroValues.put(controllerAxis, value);
			return true;
		} else {
			if (Math.abs(value - zeroValue) < CONTROLLER_DEAD_ZONE) {
				return false;
			}
			activeController = controller;
			if (value > 0) {
				switch (axisCode) {
				case 0: return axisDown(GdxKey.AXIS0_POSITIVE.keycode);
				case 1: return axisDown(GdxKey.AXIS1_POSITIVE.keycode);
				case 2: return axisDown(GdxKey.AXIS2_POSITIVE.keycode);
				case 3: return axisDown(GdxKey.AXIS3_POSITIVE.keycode);
				case 4: return axisDown(GdxKey.AXIS4_POSITIVE.keycode);
				case 5: return axisDown(GdxKey.AXIS5_POSITIVE.keycode);
				case 6: return axisDown(GdxKey.AXIS6_POSITIVE.keycode);
				case 7: return axisDown(GdxKey.AXIS7_POSITIVE.keycode);
				case 8: return axisDown(GdxKey.AXIS8_POSITIVE.keycode);
				case 9: return axisDown(GdxKey.AXIS9_POSITIVE.keycode);
				}
			} else {
				switch (axisCode) {
				case 0: return axisDown(GdxKey.AXIS0_NEGATIVE.keycode);
				case 1: return axisDown(GdxKey.AXIS1_NEGATIVE.keycode);
				case 2: return axisDown(GdxKey.AXIS2_NEGATIVE.keycode);
				case 3: return axisDown(GdxKey.AXIS3_NEGATIVE.keycode);
				case 4: return axisDown(GdxKey.AXIS4_NEGATIVE.keycode);
				case 5: return axisDown(GdxKey.AXIS5_NEGATIVE.keycode);
				case 6: return axisDown(GdxKey.AXIS6_NEGATIVE.keycode);
				case 7: return axisDown(GdxKey.AXIS7_NEGATIVE.keycode);
				case 8: return axisDown(GdxKey.AXIS8_NEGATIVE.keycode);
				case 9: return axisDown(GdxKey.AXIS9_NEGATIVE.keycode);
				}
			}
			return false;
		}
	}

	/**
	 * @see com.badlogic.gdx.controllers.ControllerListener#povMoved
	 * (com.badlogic.gdx.controllers.Controller, int, com.badlogic.gdx.controllers.PovDirection)
	 */
	@Override
	public boolean povMoved(Controller controller, int povCode, PovDirection value) {
		// think this is unused anyway?
		return false;
	}

	/**
	 * @see com.badlogic.gdx.controllers.ControllerListener#xSliderMoved
	 * (com.badlogic.gdx.controllers.Controller, int, boolean)
	 */
	@Override
	public boolean xSliderMoved(Controller controller, int sliderCode, boolean value) {
		// think this is unused anyway?
		return false;
	}

	/**
	 * @see com.badlogic.gdx.controllers.ControllerListener#ySliderMoved
	 * (com.badlogic.gdx.controllers.Controller, int, boolean)
	 */
	@Override
	public boolean ySliderMoved(Controller controller, int sliderCode, boolean value) {
		// think this is unused anyway?
		return false;
	}

	/**
	 * @see com.badlogic.gdx.controllers.ControllerListener#accelerometerMoved
	 * (com.badlogic.gdx.controllers.Controller, int, com.badlogic.gdx.math.Vector3)
	 */
	@Override
	public boolean accelerometerMoved(Controller controller, int accelerometerCode, Vector3 value) {
		// think this is unused anyway?
		return false;
	}

	/**
	 * Given a control button, returns the associated config key.
	 * @param	button			The button to get config for
	 * @return					The name of that button's config key
	 */
	public String configForButton(InputButton button) {
		switch (button) {
		case BUTTON_A:			return Constants.ARG_CONTROL_A;
		case BUTTON_B:			return Constants.ARG_CONTROL_B;
		case BUTTON_START:		return Constants.ARG_CONTROL_START;
		case BUTTON_SELECT:		return Constants.ARG_CONTROL_SELECT;
		case LEFT:				return Constants.ARG_CONTROL_LEFT;
		case RIGHT:				return Constants.ARG_CONTROL_RIGHT;
		case UP:				return Constants.ARG_CONTROL_UP;
		case DOWN:				return Constants.ARG_CONTROL_DOWN;
		default:				return "";
		}
	}
	
	/**
	 * Called every time an axis changes while down.
	 * @param	keycode			The keycode to the button corresponding
	 * @return					True to halt processing
	 */
	public boolean axisDown(int keycode) {
		keyDown(keycode);
		return true;
	}
	
	/**
	 * Signal that a meta-button event (press etc) occurred. Construct the event
	 * yourself in the raw input handling.
	 * @param 	event			The event that occurred
	 */
	protected final void signal(InputEvent event) {
		List<ButtonListener> toTrigger = new ArrayList<ButtonListener>();
		for (ButtonListener listener : buttonListeners) {
			toTrigger.add(listener);
		}
		for (ButtonListener listener : toTrigger) {
			listener.onEvent(event);
		}
	}
	
	/**
	 * Checks to see if a virtual button is pressed by any source.
	 * @param	button			The button to check
	 * @return					True if any of that button's buttons are down
	 */
	protected boolean buttonDown(InputButton button) {
		ArrayList<Integer> checkKeys = new ArrayList<Integer>();
		checkKeys.addAll(buttonToKey.get(button));
		
		String bind = MGlobal.args.get(configForButton(button));
		if (bind != null) {
			int matchingCode;
			try {
				GdxKey key = GdxKey.valueOf(bind);
				matchingCode = key.keycode;
			} catch (IllegalArgumentException e) {
				matchingCode = Integer.valueOf(bind);
			}
			checkKeys.add(matchingCode);
		}
		
		for (Integer keycode : checkKeys) {
			if (Gdx.input.isKeyPressed(keycode)) {
				return true;
			}
			
			// joystick axis hack
			if (keycode > 200 && keycode < 230 && activeController != null) {
				// suuuuper hack ugh
				int axis = (int) Math.floor((float)(keycode - 200) / 2.0f);
				float current = activeController.getAxis(axis);
				float zero = axisZeroValues.get(new ControllerAxis(activeController, axis));
				return Math.abs(current - zero) > CONTROLLER_DEAD_ZONE;
			}
			
			// joystick button hack
			if (bind != null && activeController != null) {
				if (activeController.getButton(keycode)) {
					return true;
				}
			}
		}
		return false;
	}
	
	/**
	 * Converts keycode to button.
	 * @param	keycode			The keycode to evaluate
	 * @return					The button associated with the key, or null
	 */
	protected InputButton keyToButton(int keycode) {
		for (InputButton button : InputButton.values()) {
			String bind = MGlobal.args.get(configForButton(button));
			if (bind != null) {
				int matchingCode;
				try {
					GdxKey key = GdxKey.valueOf(bind);
					matchingCode = key.keycode;
				} catch (IllegalArgumentException e) {
					matchingCode = Integer.valueOf(bind);
				}
				if (matchingCode == keycode) {
					return button;
				}
			}
		}
		return keyToButton.get(keycode);
	}
	
	/**
	 * Short thing to keep track of buttons.
	 */
	public enum KeyState {
		DOWN,
		UP,
	}
	
	protected enum AxisState {
		NEUTRAL,
		DOWN,
		REPEAT,
	}
	
	/**
	 * Controller + Axis = ControllerAxis
	 */
	protected class ControllerAxis {
		public Controller controller;
		public int axis;
		
		public ControllerAxis(Controller controller, int axis) {
			this.axis = axis;
			this.controller = controller;
		}
		
		/** @see java.lang.Object#equals(java.lang.Object) */
		@Override public boolean equals(Object otherObject) {
			if (!otherObject.getClass().equals(getClass())) {
				return false;
			}
			ControllerAxis other = (ControllerAxis) otherObject;
			return other.axis == this.axis && other.controller == this.controller;
		}

		/** @see java.lang.Object#hashCode() */
		@Override public int hashCode() {
			return Integer.valueOf(axis).hashCode() + controller.hashCode();
		}
		
	}
	
}
