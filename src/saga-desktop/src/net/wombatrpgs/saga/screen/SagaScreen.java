/**
 *  SagaScreen.java
 *  Created on May 15, 2014 1:34:09 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.screen;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.glutils.ShaderProgram;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.FinishListener;
import net.wombatrpgs.mgne.graphics.BatchWithShader;
import net.wombatrpgs.mgne.io.InputEvent;
import net.wombatrpgs.mgne.io.InputEvent.EventType;
import net.wombatrpgs.mgne.screen.Screen;
import net.wombatrpgs.mgne.screen.WindowSettings;
import net.wombatrpgs.mgneschema.io.data.InputButton;
import net.wombatrpgs.mgneschema.io.data.InputCommand;
import net.wombatrpgs.saga.core.DebugThread;
import net.wombatrpgs.saga.core.SGlobal;

/**
 * Saga-specific screen meant to apply some gameboy filters.
 */
public class SagaScreen extends Screen {
	
	protected static final float WIPE_TIME = .4f;	// in s
	
	public enum FadeType {
		TO_WHITE, TO_BLACK, FROM_WHITE, FROM_BLACK
	}
	
	public enum TransitionType {
		WHITE, BLACK
	}
	
	protected transient BatchWithShader foreground;
	protected transient BatchWithShader background;
	
	protected FinishListener onWipeFinish;
	protected float sinceWipe;
	protected FadeType fade;
	protected boolean fadeDummyFrame;
	
	/**
	 * Constructs a new screen with the saga-specific effects.
	 */
	public SagaScreen() {
		
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#update(float)
	 */
	@Override
	public void update(float elapsed) {
		super.update(elapsed);
		
		if (fadeDummyFrame == true && elapsed > 0) {
			fadeDummyFrame = false;
			return;
		}
		
		float ratio = sinceWipe / WIPE_TIME;
		float arg = 0;
		if (ratio > .75) {
			arg = 1f;
		} else if (ratio > .5) {
			arg = .66f;
		} else if (ratio > .25) {
			arg = .33f;
		}
		
		if (fade != null) {
			switch (fade) {
			case FROM_BLACK:	arg -= 1;				break;
			case FROM_WHITE:	arg = 1 - arg;			break;
			case TO_BLACK:		arg *= -1;				break;
			case TO_WHITE:								break;
			}
		}
		
		foreground.getShader().begin();
		foreground.getShader().setUniformf("elapsed", arg);
		foreground.getShader().end();
		
		background.getShader().begin();
		background.getShader().setUniformf("elapsed", arg);
		background.getShader().end();
		
		// we update /after/ the settings to avoid one-off-frame issues
		if (fade != null) {
			sinceWipe += elapsed;
			if (sinceWipe > WIPE_TIME) {
				if (onWipeFinish != null) {
					FinishListener lastListener = onWipeFinish;
					onWipeFinish = null;
					lastListener.onFinish();
				}
			}
		}
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#constructMapShader()
	 */
	@Override
	public ShaderProgram constructMapShader() {
		if (background == null) {
			background = SGlobal.graphics.constructBackgroundBatch();
		}
		return background.getShader();
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onEvent
	 * (net.wombatrpgs.mgne.io.InputEvent)
	 */
	@Override
	public void onEvent(InputEvent event) {
		if (event.button == InputButton.DEBUG && event.type == EventType.PRESS) {
			DebugThread.launchInstance();
		} else {
			super.onEvent(event);
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onCommand
	 * (net.wombatrpgs.mgneschema.io.data.InputCommand)
	 */
	@Override
	public boolean onCommand(InputCommand command) {
		if (fade != null && sinceWipe <= WIPE_TIME) return true;
		return super.onCommand(command);
	}

	/**
	 * Fades out the screen with the appropriate transition.
	 * @param	fade			The fade to/from and color
	 * @param	listener		The listener to call when transition is done
	 */
	public void fade(FadeType fade, final FinishListener listener) {
		this.fade = fade;
		sinceWipe = 0;
		onWipeFinish = listener;
		fadeDummyFrame = true;
	}
	
	/**
	 * Checks if this screen is currently fading.
	 * @return					True if a fade is active
	 */
	public boolean isFading() {
		if (fade == null) return false;
		if (sinceWipe == 0) return false;
		if (sinceWipe > WIPE_TIME) return false;
		return true;
	}
	
	/**
	 * Transitions this screen into the stack by fading out the screen below,
	 * fading in this screen, then calling the finish listener.
	 * @param	transition		The transition color during the switch
	 * @param	listener		The listener to call when done (or null)
	 */
	public void transitonOn(TransitionType transition, final FinishListener listener) {
		transition(transition, listener, false);
	}
	
	/**
	 * Transitions this screen off the stack by fading out this screen, fading
	 * in the screen below, then calling the finish listener.
	 * @param	transition		The transition color during the switch
	 * @param	listener		The listener to call when done (or null)
	 */
	public void transitonOff(TransitionType transition, final FinishListener listener) {
		transition(transition, listener, true);
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#constructUIBatch()
	 */
	@Override
	protected SpriteBatch constructUIBatch() {
		if (background == null) {
			background = SGlobal.graphics.constructBackgroundBatch();
		}
		return background.getBatch();
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#constructViewBatch()
	 */
	@Override
	protected SpriteBatch constructViewBatch() {
		if (foreground == null) {
			foreground = SGlobal.graphics.constructForegroundBatch();
		}
		return foreground.getBatch();
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#clear()
	 */
	@Override
	protected void clear() {
		WindowSettings window = MGlobal.window;
		MGlobal.graphics.getBackground().renderAt(
				background.getBatch(),
				0, 0,
				window.getWidth(), window.getHeight());
	}
	
	/**
	 * Internal transition manager with a dumb boolean for on/off.
	 * @param	type			The transition color desired
	 * @param	listener		The listener to call when done
	 * @param	remove			True to remove this screen instead
	 */
	protected void transition(TransitionType type,
			final FinishListener listener, final boolean remove) {
		FadeType out = null;
		FadeType in = null;
		switch (type) {
		case BLACK:
			out = FadeType.TO_BLACK;
			in = FadeType.FROM_BLACK;
			break;
		case WHITE:
			out = FadeType.TO_WHITE;
			in = FadeType.FROM_WHITE;
			break;
		}
		final FadeType finalIn = in;
		SagaScreen original = (SagaScreen) MGlobal.screens.peek();
		SagaScreen newScreen = this;
		final SagaScreen outScreen = remove ? newScreen : original;
		final SagaScreen inScreen = remove? original : newScreen;
		outScreen.fade(out, new FinishListener() {
			@Override public void onFinish() {
				if (remove) {
					MGlobal.screens.pop();
					SagaScreen newIn = (SagaScreen) MGlobal.screens.peek();
					newIn.fade(finalIn, listener);
					newIn.update(0);
				} else {
					MGlobal.screens.push(inScreen);
					inScreen.fade(finalIn, listener);
					inScreen.update(0);
				}
			}
		});
	}

}
