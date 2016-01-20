/**
 *  BattleAnimSound.java
 *  Created on Sep 15, 2014 10:24:15 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.graphics.banim;

import net.wombatrpgs.sagaschema.graphics.banim.BattleAnimSoundMDO;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;

/**
 * A sound-only battle animation.
 */
public class BattleAnimSound extends BattleAnim {
	
	protected BattleAnimSoundMDO mdo;
	protected float elapsed;

	/**
	 * Creates a new sound-only animation from data.
	 * @param	mdo				The data to create from
	 */
	public BattleAnimSound(BattleAnimSoundMDO mdo) {
		super(mdo);
		this.mdo = mdo;
	}

	/**
	 * @see net.wombatrpgs.mgne.graphics.interfaces.PosRenderable#renderAt
	 * (com.badlogic.gdx.graphics.g2d.SpriteBatch, float, float)
	 */
	@Override
	public void renderAt(SpriteBatch batch, float x, float y) {
		// no appearance
	}

	/**
	 * @see net.wombatrpgs.saga.graphics.PortraitAnim#isDone()
	 */
	@Override
	public boolean isDone() {
		return elapsed > mdo.duration;
	}

	/**
	 * @see net.wombatrpgs.saga.graphics.PortraitAnim#update(float)
	 */
	@Override
	public void update(float elapsed) {
		super.update(elapsed);
		this.elapsed += elapsed;
	}

}
