/**
 *  EffectTeleport.java
 *  Created on Jul 12, 2015 5:16:23 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.warheads;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.maps.TeleportMemory;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.saga.screen.ScreenWorld;
import net.wombatrpgs.saga.screen.TargetSelectable;
import net.wombatrpgs.saga.ui.TeleportSelector;
import net.wombatrpgs.sagaschema.rpg.abil.data.AbilEffectMDO;

/**
 * Teleport to any memorized location.
 */
public class EffectTeleport extends EffectBattleUnusable {

	/**
	 * Inherited constructor.
	 * @param	mdo				The data to create from
	 * @param	item			The item to create for
	 */
	public EffectTeleport(AbilEffectMDO mdo, CombatItem item) {
		super(mdo, item);
	}

	/** @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#isMapUsable() */
	@Override public boolean isMapUsable() { return true; }

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#onMapUse
	 * (net.wombatrpgs.saga.screen.TargetSelectable)
	 */
	@Override
	public void onMapUse(TargetSelectable caller) {
		TeleportSelector selector = new TeleportSelector();
		MGlobal.assets.loadAsset(selector, "teleport selector");
		selector.selectLocation(selector.new TeleportSelectionListener() {
			@Override public void onFinish(TeleportMemory memory) {
				if (memory != null) {
					// TODO: polish: animate this
					MGlobal.audio.playSFX(SConstants.SFX_SAVE);
					// ugly stuff to go back to the world screen
					while (!ScreenWorld.class.isAssignableFrom(MGlobal.screens.peek().getClass())) {
						MGlobal.screens.pop();
					}
					MGlobal.levelManager.getTele().teleport(memory.mapKey,
							memory.tileX,
							memory.tileY);
					item.deductUse();
				}
			}
		});
	}

}
