/**
 *  ScreenMovable.java
 *  Created on Jan 17, 2014 10:52:05 PM for project saga
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.graphics;

import com.badlogic.gdx.graphics.Color;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;

import net.wombatrpgs.mgne.maps.PositionSetable;
import net.wombatrpgs.mgne.screen.Screen;
import net.wombatrpgs.mgne.screen.ScreenObject;

/**
 * Somewhere between a picture and an object. This is so text objects can get
 * the picture positioning stuff. Also has a batch because uh yeah you're going
 * to be rendering things.
 */
public abstract class ScreenDrawable extends ScreenObject implements PositionSetable {
	
	protected SpriteBatch batch;
	protected Screen parent;
	protected Color currentColor;
	protected float x, y;
	protected boolean fadingOut;
	
	protected boolean tweening;
	protected Color tweenTargetColor;
	protected Color tweenBaseColor;
	protected float tweenTime;
	protected float tweenEnd;

	/**
	 * Creates a new screen object with z blah blah superconstructor.
	 * @param	z				The z-coord of the picture, greater is higher
	 */
	public ScreenDrawable(int z) {
		super(z);
		this.tweening = false;
		this.batch = new SpriteBatch();
		this.currentColor = new Color(1, 1, 1, 1);
		this.tweenBaseColor = new Color(1, 1, 1, 1);
		this.tweenTargetColor = new Color(1, 1, 1, 1);
	}

	/** @see net.wombatrpgs.mgne.maps.Positionable#getX() */
	@Override public float getX() { return this.x; }
	
	/** @see net.wombatrpgs.mgne.maps.Positionable#getY() */
	@Override public float getY() {	return this.y; }
	
	/** @see net.wombatrpgs.mgne.maps.PositionSetable#setX(int) */
	@Override public void setX(float x) { this.x = x; }

	/** @see net.wombatrpgs.mgne.maps.PositionSetable#setY(int) */
	@Override public void setY(float y) { this.y = y; }
	
	/** @return True if we're currently moving to another place/color */
	public boolean isTweening() { return tweening; }
	
	/**
	 * @see net.wombatrpgs.mgne.core.interfaces.Updateable#update(float)
	 */
	@Override
	public void update(float elapsed) {
		if (fadingOut && !isTweening()) {
			parent.removeObject(this);
		}
		if (tweening) {
			tweenTime += elapsed;
			float r;
			if (tweenTime > tweenEnd) {
				r = 1;
				tweening = false;
			} else {
				r = tweenTime / tweenEnd;
			}
			currentColor.a = tweenBaseColor.a * (1.f-r) + r * tweenTargetColor.a;
			currentColor.r = tweenBaseColor.r * (1.f-r) + r * tweenTargetColor.r;
			currentColor.g = tweenBaseColor.g * (1.f-r) + r * tweenTargetColor.g;
			currentColor.b = tweenBaseColor.b * (1.f-r) + r * tweenTargetColor.b;
			batch.setColor(currentColor);
		}
	}
	
	/**
	 * Gets the default screen batch. This is usually what's used to render,
	 * although if some special thing is going on, it'd be best to use a
	 * different batch.
	 * @return					The current screen's sprite batch
	 */
	public SpriteBatch getBatch() {
		return batch;
	}
	
	/**
	 * Sets the batch we use to render. This is kind of hacky and shouldn't be
	 * used.
	 * @param 	batch			The batch to set to
	 */
	public void setBatch(SpriteBatch batch) {
		this.batch = batch;
	}
	
	/**
	 * Tweens to a new color. Undefined behavior if already tweening. This is a
	 * smooth transition from one color to another.
	 * @param 	target			The color to end up as
	 * @param 	time			How long to take
	 */
	public void tweenTo(Color target, float time) {
		tweenTargetColor.set(target);
		tweenBaseColor.set(currentColor);
		this.tweenEnd = time;
		this.tweenTime = 0;
		this.tweening = true;
	}
	
	/**
	 * Causes the text box to fade in to the current screen. Clears any text on
	 * the box.
	 * @param	screen			The screen to fade in on
	 * @param	fadeTime		How long the transition should take
	 */
	public void fadeIn(Screen screen, float fadeTime) {
		this.parent = screen;
		setColor(new Color(1, 1, 1, 0));
		if (!screen.containsChild(this)) {
			screen.addObject(this);
		}
		tweenTo(new Color(1, 1, 1, 1), fadeTime);
	}
	
	/**
	 * Gracefully exits from the screen.
	 * @param	fadeTime		How long the transition should take
	 */
	public void fadeOut(float fadeTime) {
		tweenTo(new Color(1, 1, 1, 0), fadeTime);
		fadingOut = true;
	}
	
	/**
	 * Sets the picture's rendering color.
	 * @param 	newColor
	 */
	public void setColor(Color newColor) {
		tweening = false;
		currentColor.set(newColor);
		batch.setColor(newColor);
	}

}