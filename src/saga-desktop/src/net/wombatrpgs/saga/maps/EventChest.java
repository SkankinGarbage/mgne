/**
 *  EventChest.java
 *  Created on Sep 12, 2014 1:34:04 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.maps;

import org.luaj.vm2.LuaValue;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.graphics.FacesAnimation;
import net.wombatrpgs.mgne.graphics.FacesAnimationFactory;
import net.wombatrpgs.mgne.maps.TiledMapObject;
import net.wombatrpgs.mgne.maps.events.MapEvent;
import net.wombatrpgs.mgne.scenes.SceneParser;
import net.wombatrpgs.mgne.scenes.StringSceneParser;
import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.battle.Battle;
import net.wombatrpgs.saga.rpg.chara.EnemyParty;
import net.wombatrpgs.saga.rpg.items.Collectable;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.sagaschema.events.EventChestMDO;
import net.wombatrpgs.sagaschema.events.data.ChestInvisibilityType;
import net.wombatrpgs.sagaschema.events.data.KeyItemType;
import net.wombatrpgs.sagaschema.rpg.abil.CombatItemMDO;
import net.wombatrpgs.sagaschema.rpg.encounter.EncounterMDO;

/**
 * Because scripting them is just... *effort*.
 */
public class EventChest extends MapEvent {
	
	protected static String KEY_ANIM_CLOSED = "anim_chest_closed";
	protected static String KEY_ANIM_OPEN = "anim_chest_open";
	
	protected EventChestMDO mdo;
	protected FacesAnimation openSprite, closedSprite;
	protected String switchName;
	protected CombatItem item;
	protected Collectable collectable;
	protected Battle encounter;
	protected SceneParser scene;
	protected boolean keyItem, invisible;
	
	/**
	 * Creates a new event from data.
	 * @param	mdo					The data to create from
	 * @param	object				The raw object, has some extra info we need
	 */
	public EventChest(EventChestMDO mdo, TiledMapObject object) {
		super(mdo);
		openSprite = FacesAnimationFactory.create(KEY_ANIM_OPEN);
		closedSprite = FacesAnimationFactory.create(KEY_ANIM_CLOSED);
		assets.add(openSprite);
		assets.add(closedSprite);
		
		if (mdoHasProperty(mdo.item)) {
			item = new CombatItem(MGlobal.data.getEntryFor(mdo.item, CombatItemMDO.class));
			assets.add(item);
		} else if (mdoHasProperty(mdo.collectable)) {
			collectable = new Collectable(mdo.collectable);
		} else if (mdoHasProperty(mdo.encounter)) {
			EncounterMDO encounterMDO = MGlobal.data.getEntryFor(mdo.encounter, EncounterMDO.class);
			EnemyParty party = new EnemyParty(encounterMDO);
			encounter = new Battle(party, true);
			encounter.setFleeable(false);
		}
		
		if (mdoHasProperty(mdo.switchName)) {
			switchName = mdo.switchName;
		} else {
			switchName = "chest_" + object.getLevel().getKeyName();
			switchName += "(" + object.getTileX() + "," + object.getTileY() + ")";
		}
		
		keyItem = (mdo.keyItem == KeyItemType.KEY_ITEM);
		invisible = (mdo.visible == ChestInvisibilityType.INVISIBLE);
		if (mdo.scene != null && !mdo.scene.isEmpty()) {
			scene = new StringSceneParser(mdo.scene, LuaValue.NIL);
			assets.add(scene);
		}
		
		setAppearance();
	}

	/**
	 * @see net.wombatrpgs.mgne.maps.events.MapEvent#onInteract()
	 */
	@Override
	public boolean onInteract() {
		if (!chestOpened() && !isHidden()) {
			if (encounter != null) {
				MGlobal.assets.loadAsset(encounter, "chest encounter");
				MGlobal.memory.setSwitch(switchName);
				setAppearance();
				MGlobal.audio.playSFX(SConstants.SFX_BATTLE);
				encounter.start();
			} else if (item != null) {
				if (SGlobal.heroes.getInventory().isFull()) {
					MGlobal.ui.getBlockingBox().blockText(
							MGlobal.levelManager.getScreen(), "No more room for items!");
				} else {
					if (keyItem) {
						MGlobal.ui.getBlockingBox().blockText(
								MGlobal.levelManager.getScreen(),
								"Retrieved the " + item.getName() + ".");
					} else {
						MGlobal.ui.getBlockingBox().blockText(
								MGlobal.levelManager.getScreen(),
								"Retrieved a " + item.getName() + ".");
					}
					SGlobal.heroes.getInventory().add(item);
					MGlobal.memory.setSwitch(switchName);
					MGlobal.audio.playSFX(SConstants.SFX_GET);
					setAppearance();
				}
			} else if (collectable != null) {
				SGlobal.heroes.getCollection().addCollectable(collectable);
				MGlobal.memory.setSwitch(switchName);
				MGlobal.audio.playSFX(SConstants.SFX_GET);
				String chestName = collectable.getChestName();
				if (chestName == null || chestName.isEmpty()) {
					MGlobal.ui.getBlockingBox().blockText(
							MGlobal.levelManager.getScreen(),
							"Retrieved a " + collectable.getName() + ".");
				} else {
					int quantity = SGlobal.heroes.getCollection().getQuantity(collectable);
					MGlobal.ui.getBlockingBox().blockText(
							MGlobal.levelManager.getScreen(),
							"Retrieved part " + quantity + " of " + chestName + ".");
				}
				setAppearance();
			} else {
				MGlobal.ui.getBlockingBox().blockText(
						MGlobal.levelManager.getScreen(), "Empty.");
				MGlobal.memory.setSwitch(switchName);
				setAppearance();
			}
			if (scene != null) {
				scene.run();
			}
		}
		return true;
	}

	/**
	 * @see net.wombatrpgs.mgne.maps.events.MapEvent#update(float)
	 */
	@Override
	public void update(float elapsed) {
		super.update(elapsed);
		setAppearance();
	}

	/**
	 * @see net.wombatrpgs.mgne.maps.events.MapEvent#isPassable()
	 */
	@Override
	public boolean isPassable() {
		return isHidden();
	}

	/**
	 * Sets appearance based on switch status.
	 */
	protected void setAppearance() {
		if (chestOpened()) {
			setAppearance(openSprite);
		} else if (!invisible) {
			setAppearance(closedSprite);
		}
	}
	
	/**
	 * Checks if this chest should appear open or closed.
	 * @return					True if the chest should be open
	 */
	protected boolean chestOpened() {
		if (keyItem) {
			return SGlobal.heroes.isCarryingItemType(item);
		} else {
			return MGlobal.memory.getSwitch(switchName);
		}
	}
}
