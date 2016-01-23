/**
 *  EffectUseRestore.java
 *  Created on Oct 18, 2015 1:04:18 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.warheads;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.screen.Screen;
import net.wombatrpgs.mgne.ui.Nineslice;
import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.saga.screen.TargetSelectable;
import net.wombatrpgs.saga.ui.ItemSelector;
import net.wombatrpgs.saga.ui.SlotListener;
import net.wombatrpgs.sagaschema.rpg.abil.data.AbilEffectMDO;

/**
 * Item use restoration effect.
 */
public class EffectUseRestore extends EffectBattleUnusable {
	
	protected static final int ITEMS_WIDTH = 128;
	protected static final int ITEMS_LIST_PADDING = 3;

	/**
	 * Inherited constructor.
	 * @param	mdo				The data to create item for
	 * @param	item			The item to create effect for
	 */
	public EffectUseRestore(AbilEffectMDO mdo, CombatItem item) {
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
		final ItemSelector selector = new ItemSelector(SGlobal.heroes.getInventory(),
				SGlobal.heroes.getInventory().slotCount(), ITEMS_WIDTH,
				ITEMS_LIST_PADDING, false, false) {
			Nineslice bg = new Nineslice(getWidth() + 16, getHeight() + 20);
			/* initializer */ {
				MGlobal.assets.loadAsset(bg, "arcane bg");
			}
			/** @see net.wombatrpgs.saga.ui.ItemSelector#coreRender(com.badlogic.gdx.graphics.g2d.SpriteBatch) */
			@Override public void coreRender(SpriteBatch batch) {
				bg.renderAt(batch, getX() - 8, getY() - 2);
				super.coreRender(batch);
			}
		};
		final Screen screen = MGlobal.screens.peek();
		selector.setX((screen.getWidth() - selector.getWidth()) / 2);
		selector.setY((screen.getHeight() - selector.getHeight()) / 2);
		MGlobal.assets.loadAsset(selector, "arcane selector");
		screen.addChild(selector);
		selector.awaitSelection(new SlotListener() {
			@Override public boolean onSelection(int selected) {
				if (selected < 0) {
					screen.removeChild(selector);
					return true;
				}
				CombatItem restoreItem = SGlobal.heroes.getInventory().get(selected);
				if (restoreItem == null || restoreItem.isUnlimited() || restoreItem == item) {
					MGlobal.audio.playSFX(SConstants.SFX_FAIL);
					screen.removeChild(selector);
					return true;
				}
				restoreItem.restoreUsesArcane();
				item.deductUse();
				MGlobal.audio.playSFX(SConstants.SFX_CURE);
				screen.removeChild(selector);
				return true;
			}
		}, true);
	}

}
