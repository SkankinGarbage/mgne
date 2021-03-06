/**
 *  BattleAnimationFactory.java
 *  Created on May 23, 2014 8:39:29 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.graphics.banim;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.sagaschema.graphics.banim.BattleAnimSeriesMDO;
import net.wombatrpgs.sagaschema.graphics.banim.BattleAnimShaderMDO;
import net.wombatrpgs.sagaschema.graphics.banim.BattleAnimShotsMDO;
import net.wombatrpgs.sagaschema.graphics.banim.BattleAnimSoundMDO;
import net.wombatrpgs.sagaschema.graphics.banim.BattleAnimStripMDO;
import net.wombatrpgs.sagaschema.graphics.banim.data.BattleAnimMDO;

/**
 * Creates battle animations from MDOs.
 */
public class BattleAnimFactory {
	
	/**
	 * Creates a battle animation from data.
	 * @param	mdo				The data to create animation from
	 * @return					The appropriate subclass instance of the anim
	 */
	public static BattleAnim create(BattleAnimMDO mdo) {
		if (BattleAnimStripMDO.class.isAssignableFrom(mdo.getClass())) {
			return new BattleAnimStrip((BattleAnimStripMDO) mdo);
		} else if (BattleAnimSeriesMDO.class.isAssignableFrom(mdo.getClass())) {
			return new BattleAnimSeries((BattleAnimSeriesMDO) mdo);
		} else if (BattleAnimShotsMDO.class.isAssignableFrom(mdo.getClass())) {
			return new BattleAnimShots((BattleAnimShotsMDO) mdo);
		} else if (BattleAnimShaderMDO.class.isAssignableFrom(mdo.getClass())) {
			return new BattleAnimShader((BattleAnimShaderMDO) mdo);
		} else if (BattleAnimSoundMDO.class.isAssignableFrom(mdo.getClass())) {
			return new BattleAnimSound((BattleAnimSoundMDO) mdo);
		} else {
			MGlobal.reporter.err("Unknown batle anim type: " + mdo);
			return null;
		}
	}
	
	/**
	 * Creates a battle animation from the key to some data.
	 * @param	key				The key to the data to create animation from
	 * @return					The appropriate subclass instance of the anim
	 */
	public static BattleAnim create(String key) {
		return create(MGlobal.data.getEntryFor(key, BattleAnimMDO.class));
	}

}
