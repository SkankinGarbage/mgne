/**
 *  EffectTransform.java
 *  Created on Jul 19, 2015 5:27:01 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.warheads;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.FinishListener;
import net.wombatrpgs.mgne.graphics.FacesAnimation;
import net.wombatrpgs.mgne.graphics.FacesAnimationFactory;
import net.wombatrpgs.mgne.maps.MapThing;
import net.wombatrpgs.mgne.maps.objects.TimerListener;
import net.wombatrpgs.mgne.maps.objects.TimerObject;
import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.saga.screen.TargetSelectable;
import net.wombatrpgs.sagaschema.rpg.warheads.EffectTransformMDO;

/**
 * Temporarily changes the party's sprite and sets a switch.
 */
public class EffectTransform extends EffectBattleUnusable implements FinishListener {
	
	protected static String PROPERTY_PRESERVES_TRANSFORM = "preservesTransform";
	
	protected EffectTransformMDO mdo;

	/**
	 * Inherited constructor.
	 * @param	mdo				The data to construct the effect from
	 * @param	item			The item to construct for
	 */
	public EffectTransform(EffectTransformMDO mdo, CombatItem item) {
		super(mdo, item);
		this.mdo = mdo;
	}

	/** @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#isMapUsable() */
	@Override public boolean isMapUsable() { return true; }

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#onMapUse
	 * (net.wombatrpgs.saga.screen.TargetSelectable)
	 */
	@Override
	public void onMapUse(TargetSelectable caller) {
		MGlobal.audio.playSFX(SConstants.SFX_SAVE);
		FacesAnimation appearance = FacesAnimationFactory.create(mdo.sprite);
		MGlobal.assets.loadAsset(appearance, "transform appearance");
		MGlobal.getHero().setAppearance(appearance);
		MGlobal.memory.switches.put(mdo.switchName, true);
		MGlobal.levelManager.getTele().getPre().addListener(this);
	}

	/**
	 * @see net.wombatrpgs.mgne.core.interfaces.FinishListener#onFinish()
	 */
	@Override
	public void onFinish() {
		String value = MGlobal.levelManager.getActive().getProperty(PROPERTY_PRESERVES_TRANSFORM);
		if (!MapThing.mdoHasProperty(value)) {
			SGlobal.heroes.setLeaderAppearance();
			MGlobal.memory.switches.put(mdo.switchName, false);
		} else {
			final EffectTransform effect = this;
			new TimerObject(0.0f, MGlobal.screens.peek(), new TimerListener() {
				@Override public void onTimerZero(TimerObject source) {
					MGlobal.levelManager.getTele().getPre().addListener(effect);
				}
			});			
		}
	}

}
