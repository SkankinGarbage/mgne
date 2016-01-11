/**
 *  ShopScreen.java
 *  Created on May 20, 2014 2:24:37 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.screen;

import java.util.ArrayList;
import java.util.List;

import com.badlogic.gdx.graphics.g2d.BitmapFont.HAlignment;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.FinishListener;
import net.wombatrpgs.mgne.ui.Nineslice;
import net.wombatrpgs.mgne.ui.Option;
import net.wombatrpgs.mgne.ui.OptionSelector;
import net.wombatrpgs.mgne.ui.text.FontHolder;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.saga.rpg.items.ShopInventory;
import net.wombatrpgs.saga.ui.ItemSelector;
import net.wombatrpgs.saga.ui.SlotListener;
import net.wombatrpgs.sagaschema.rpg.abil.CombatItemMDO;
import net.wombatrpgs.sagaschema.rpg.shop.ShopMDO;

/**
 * Shopping is my favorite hobby, even more than gam mak.
 */
public class ScreenItemShop extends ScreenShop {
	
	protected static final int ITEMS_WIDTH = 188;
	protected static final int ITEMS_EDGE_PADDING = 12;
	protected static final int ITEMS_LIST_PADDING = 3;
	protected static final int ITEMS_PRICE_WIDTH = 56;
	protected static final int HEADER_HEIGHT = 32;
	protected static final int HEADER_MARGINS = 8;
	protected static final int GLOBAL_Y = 80;
	
	protected ShopInventory shop;
	protected ItemSelector shopSelector, partySelector;
	protected ItemSelector shopViewer, partyViewer;
	protected Nineslice shopBG, partyBG;
	protected Nineslice shopViewBG, partyViewBG;
	protected Nineslice headerBG;
	protected TextFormat headerFormat;
	protected String headerString;
	protected float globalX, globalY;
	protected float shopX, shopY, partyX, partyY;
	protected float shopViewX, shopViewY, partyViewX, partyViewY;
	protected float headerX, headerY;
	
	protected boolean sellMode;
	
	/**
	 * Creates a shop that carries the items corresponding to the provided
	 * item mdos.
	 * @param	mdos			The list of MDOs of items in the shop
	 */
	public ScreenItemShop(List<CombatItemMDO> mdos) {
		shop = new ShopInventory(mdos);
		
		menu = new OptionSelector(
				new Option("Buy") {
					@Override public boolean onSelect() { return onBuy(); }
				},
				new Option("Sell") {
					@Override public boolean onSelect() { return onSell(); }
				},
				new Option("Leave") {
					@Override public boolean onSelect() { return onLeave(); }
				});
		menu.setCancel(new FinishListener() {
				@Override public void onFinish() {
					onLeave();
				}
		});
		assets.add(menu);
		
		shopSelector = new ItemSelector(shop, shop.slotCount(),
				ITEMS_WIDTH - ITEMS_EDGE_PADDING * 2, ITEMS_LIST_PADDING,
				false, true);
		partySelector = new ItemSelector(SGlobal.heroes.getInventory(),
				SGlobal.heroes.getInventory().slotCount(),
				ITEMS_WIDTH - ITEMS_EDGE_PADDING * 2,
				ITEMS_LIST_PADDING, false, true);
		shopViewer = new ItemSelector(shop, shop.slotCount(),
				shopSelector.getWidth() - ITEMS_PRICE_WIDTH,
				ITEMS_LIST_PADDING, false, false);
		partyViewer = new ItemSelector(SGlobal.heroes.getInventory(),
				SGlobal.heroes.getInventory().slotCount(),
				partySelector.getWidth() - ITEMS_PRICE_WIDTH,
				 ITEMS_LIST_PADDING, false, false);
		assets.add(shopSelector);
		assets.add(partySelector);
		assets.add(shopViewer);
		assets.add(partyViewer);
		
		shopBG = new Nineslice(ITEMS_WIDTH, shopSelector.getHeight() + ITEMS_EDGE_PADDING * 2);
		partyBG = new Nineslice(ITEMS_WIDTH, shopSelector.getHeight() + ITEMS_EDGE_PADDING * 2);
		shopViewBG = new Nineslice(shopBG.getWidth() - ITEMS_PRICE_WIDTH, shopBG.getHeight());
		partyViewBG = new Nineslice(partyBG.getWidth() - ITEMS_PRICE_WIDTH, partyBG.getHeight());
		headerBG = new Nineslice(ITEMS_WIDTH * 2 - shopBG.getBorderWidth() - ITEMS_PRICE_WIDTH, HEADER_HEIGHT);
		assets.add(shopBG);
		assets.add(partyBG);
		assets.add(shopViewBG);
		assets.add(partyViewBG);
		assets.add(headerBG);
		
		globalX = (getWidth() - (ITEMS_WIDTH * 2 - shopBG.getBorderWidth() - ITEMS_PRICE_WIDTH)) / 2;
		globalY = GLOBAL_Y;
		
		shopSelector.setX(globalX + ITEMS_EDGE_PADDING);
		shopSelector.setY(globalY + ITEMS_EDGE_PADDING - 5);
		partySelector.setX(globalX + ITEMS_WIDTH - shopBG.getBorderWidth() + ITEMS_EDGE_PADDING - ITEMS_PRICE_WIDTH);
		partySelector.setY(globalY + ITEMS_EDGE_PADDING - 5);
		shopViewer.setX(shopSelector.getX());
		shopViewer.setY(shopSelector.getY());
		partyViewer.setX(partySelector.getX() + ITEMS_PRICE_WIDTH);
		partyViewer.setY(partySelector.getY());
		
		shopX = globalX;
		shopY = globalY;
		partyX = globalX + ITEMS_WIDTH - shopBG.getBorderWidth() - ITEMS_PRICE_WIDTH;
		partyY = globalY;
		shopViewX = shopX;
		shopViewY = shopY;
		partyViewX = partyX + ITEMS_PRICE_WIDTH;
		partyViewY = partyY;
		headerX = globalX;
		headerY = globalY + shopBG.getHeight() - shopBG.getBorderHeight();
		
		headerFormat = new TextFormat();
		headerFormat.align = HAlignment.LEFT;
		headerFormat.width = ITEMS_WIDTH * 2 - shopBG.getBorderWidth() * 3 - ITEMS_PRICE_WIDTH;
		headerFormat.height = HEADER_HEIGHT;
		headerFormat.x = (int) headerX + HEADER_MARGINS + headerBG.getBorderWidth() - 3;
		headerFormat.y = (int) headerY + HEADER_MARGINS + headerBG.getBorderHeight() + 11;
		headerString = "Welcome.";
	}
	
	/**
	 * Create a shop from an MDO rather than list of item MDOs.
	 * @param	mdo				The data to create from
	 */
	public ScreenItemShop(ShopMDO mdo) {
		this(extractMDOs(mdo));
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onFocusGained()
	 */
	@Override
	public void onFocusGained() {
		super.onFocusGained();
		refocusMain();
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#render(com.badlogic.gdx.graphics.g2d.SpriteBatch)
	 */
	@Override
	public void render(SpriteBatch batch) {
		super.render(batch);
		if (sellMode) {
			shopViewBG.renderAt(batch, shopViewX, shopViewY);
			partyBG.renderAt(batch, partyX, partyY);
			shopViewer.render(batch);
			partySelector.render(batch);
		} else {
			partyViewBG.renderAt(batch, partyViewX, partyViewY);
			shopBG.renderAt(batch, shopX, shopY);
			shopSelector.render(batch);
			partyViewer.render(batch);
		}
		
		headerBG.renderAt(batch, headerX, headerY);
		FontHolder font = MGlobal.ui.getFont();
		font.draw(batch, headerFormat, headerString, 0);
	}
	
	/**
	 * Peels a list of combat item MDOs out of the shop mdo's list of keys.
	 * @param	mdo				The shop MDO to extract from
	 * @return					The list of combat item MDOs in the shop
	 */
	protected static List<CombatItemMDO> extractMDOs(ShopMDO mdo) {
		List<CombatItemMDO> mdos = new ArrayList<CombatItemMDO>();
		for (String itemKey : mdo.items) {
			mdos.add(MGlobal.data.getEntryFor(itemKey, CombatItemMDO.class));
		}
		return mdos;
	}
	
	/**
	 * Called when the user elects to buy from the shop.
	 * @return					False to keep the menu open
	 */
	protected boolean onBuy() {
		menu.unfocus();
		sellMode = false;
		shopSelector.awaitSelection(new SlotListener() {
			@Override public boolean onSelection(int selected) {
				if (selected == -1) {
					refocusMain();
					return true;
				}
				if (SGlobal.heroes.getInventory().isFull()) {
					headerString = "No room.";
					return false;
				}
				CombatItem item = shop.get(selected);
				if (item == null) {
					return false;
				}
				if (SGlobal.heroes.getGP() < item.getCost()) {
					headerString = "Not enough GP.";
					return false;
				}
				shop.buyAt(selected);
				return false;
			}
		}, true);
		shopSelector.attachHoverListener(new SlotListener() {
			@Override public boolean onSelection(int selected) {
				CombatItem selectedItem = shop.get(selected);
				if (selectedItem != null) {
					headerString = selectedItem.getDescription();
				}
				return false;
			}
		});
		return false;
	}
	
	/**
	 * Called when the user elects to sell to the shop.
	 * @return					False to keep the menu open
	 */
	protected boolean onSell() {
		menu.unfocus();
		sellMode = true;
		headerString = "Sell what?";
		partySelector.awaitSelection(new SlotListener() {
			@Override public boolean onSelection(int selected) {
				if (selected == -1) {
					refocusMain();
					return true;
				} else {
					SGlobal.heroes.getInventory().sellAt(selected);
					return false;
				}
			}
		}, true);
		return false;
	}
	
	/**
	 * Shows the buy/sell/leave menu.
	 */
	protected void refocusMain() {
		sellMode = false;
		if (containsChild(menu)) {
			menu.focus();
		} else {
			menu.showAt(0, 0);
		}
	}

}
