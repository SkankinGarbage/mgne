/**
 *  ScreenMeatShop.java
 *  Created on Jan 10, 2016 1:55:18 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.screen;

import java.util.ArrayList;
import java.util.List;

import com.badlogic.gdx.utils.Align;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.FinishListener;
import net.wombatrpgs.mgne.core.interfaces.Queueable;
import net.wombatrpgs.mgne.graphics.FacesAnimation;
import net.wombatrpgs.mgne.io.CommandListener;
import net.wombatrpgs.mgne.io.CommandMap;
import net.wombatrpgs.mgne.io.command.CMapMenu;
import net.wombatrpgs.mgne.ui.Nineslice;
import net.wombatrpgs.mgne.ui.Option;
import net.wombatrpgs.mgne.ui.OptionSelector;
import net.wombatrpgs.mgne.ui.text.FontHolder;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.mgneschema.io.data.InputCommand;
import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.chara.Chara;
import net.wombatrpgs.saga.rpg.chara.MonsterFamily;
import net.wombatrpgs.sagaschema.rpg.chara.CharaMDO;
import net.wombatrpgs.sagaschema.rpg.shop.MeatShopMDO;
import net.wombatrpgs.sagaschema.rpg.shop.data.MeatShopEntryMDO;

/**
 * Nothing like a trip the the butcher.
 */
public class ScreenMeatShop extends ScreenShop {
	
	protected static final int INFO_HEIGHT = 24;
	protected static final int ITEMS_HEIGHT = 174;
	protected static final int BORDER_OVERLAP = 4;
	protected static final int MEAT_MARGIN_VERT = 24;
	protected static final int MEAT_MARGIN_HORIZ = 21;
	protected static final int TEXT_HEIGHT = 12;
	protected static final int SPECIES_ENTRY_HEIGHT = 28;
	protected static final int CURSOR_SPACING = 6;
	
	protected MeatShopMDO mdo;
	protected List<Chara> monsters;
	protected List<FacesAnimation> monsterAnims;
	protected List<FacesAnimation> heroAnims;
	protected String info;
	
	protected CommandMap map;
	protected CommandListener listener;
	
	protected TextFormat monsterSpeciesFormat, infoFormat;
	protected Nineslice meatsBacker, partyBacker, infoBacker;
	protected int meatIndex, charaIndex;
	protected boolean selectingMeat, selectingTarget;
	protected boolean done;
	
	/**
	 * Creates a new meat shop screen with the provided inventory.
	 * @param	mdo				The data to create with
	 */
	public ScreenMeatShop(final MeatShopMDO mdo) {
		this.mdo = mdo;
		
		menu = new OptionSelector(
				new Option("Buy") {
					@Override public boolean onSelect() { return onBuy(); }
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
		
		monsters = new ArrayList<Chara>();
		monsterAnims = new ArrayList<FacesAnimation>();
		for (MeatShopEntryMDO entryMDO : mdo.entries) {
			Chara monster = new Chara(entryMDO.monster);
			monsters.add(monster);
			assets.add(monster);
			
			FacesAnimation anim = monster.createSprite();
			monsterAnims.add(anim);
			assets.add(anim);
		}
		
		heroAnims = new ArrayList<FacesAnimation>();
		generateHeroSprites();
		
		int backerWidth = getWidth() / 2 + BORDER_OVERLAP;
		meatsBacker = new Nineslice(backerWidth, ITEMS_HEIGHT);
		partyBacker = new Nineslice(backerWidth, ITEMS_HEIGHT);
		assets.add(meatsBacker);
		assets.add(partyBacker);
		
		infoBacker = new Nineslice(getWidth(), INFO_HEIGHT);
		assets.add(infoBacker);
		
		monsterSpeciesFormat = new TextFormat();
		monsterSpeciesFormat.align = Align.left;
		monsterSpeciesFormat.height = TEXT_HEIGHT;
		monsterSpeciesFormat.width = getWidth();
		monsterSpeciesFormat.x = MEAT_MARGIN_HORIZ + 22;
		monsterSpeciesFormat.y = getHeight() - MEAT_MARGIN_VERT - INFO_HEIGHT + 7;
		
		infoFormat = new TextFormat();
		infoFormat.x = 9;
		infoFormat.y = getHeight() - INFO_HEIGHT + 16;
		infoFormat.align = Align.left;
		infoFormat.width = getWidth();
		infoFormat.height = TEXT_HEIGHT;
		
		resetInfo();
		
		map = new CMapMenu();
		listener = new CommandListener() {
			@Override public boolean onCommand(InputCommand command) {
				switch (command) {
				case UI_FINISH:
				case UI_CANCEL:
					if (selectingTarget) {
						selectingTarget = false;
						selectingMeat = true;
						info = "Which meat?";
					} else {
						selectingMeat = false;
						menu.focus();
						resetInfo();
						removeCommandContext(map);
						removeCommandListener(this);
					}
					return true;
				case UI_CONFIRM:
					if (selectingTarget) {
						attemptTransaction();
						selectingMeat = false;
						selectingTarget = false;
						menu.focus();
						removeCommandContext(map);
						removeCommandListener(this);
					} else {
						int price = mdo.entries[meatIndex].price;
						if (SGlobal.heroes.getGP() >= price) {
							charaIndex = 0;
							selectingTarget = true;
							selectingMeat = false;
							info = "Who eats?";
						} else {
							info = "Not enough GP.";
						}
					}
					return true;
				case MOVE_UP:
					moveCursor(-1);
					return true;
				case MOVE_DOWN:
					moveCursor(1);
					return true;
				default:
					return false;
					
				}
			}
		};
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onFocusGained()
	 */
	@Override
	public void onFocusGained() {
		super.onFocusGained();
		menu.showAt(0, 0);
		resetInfo();
	}
	
	/**
	 * @see net.wombatrpgs.saga.screen.SagaScreen#update(float)
	 */
	@Override
	public void update(float elapsed) {
		super.update(elapsed);
		for (FacesAnimation anim : heroAnims) {
			anim.update(elapsed);
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#render(com.badlogic.gdx.graphics.g2d.SpriteBatch)
	 */
	@Override
	public void render(SpriteBatch batch) {
		super.render(batch);
		
		if (selectingTarget) {
			meatsBacker.renderAt(batch, 0, getHeight() - ITEMS_HEIGHT);
			partyBacker.renderAt(batch, getWidth() - partyBacker.getWidth(), getHeight() - ITEMS_HEIGHT);
		} else {
			partyBacker.renderAt(batch, getWidth() - partyBacker.getWidth(), getHeight() - ITEMS_HEIGHT);
			meatsBacker.renderAt(batch, 0, getHeight() - ITEMS_HEIGHT);
		}
		
		FontHolder font = MGlobal.ui.getFont();
		infoBacker.renderAt(batch, 0, getHeight() - INFO_HEIGHT);
		font.draw(batch, infoFormat, info, 0);
		
		for (int i = 0; i < monsters.size(); i += 1) {
			Chara monster = monsters.get(i);
			FacesAnimation anim = monsterAnims.get(i);
			int price = mdo.entries[i].price;
			
			int y = getHeight() - (MEAT_MARGIN_VERT + INFO_HEIGHT + i * SPECIES_ENTRY_HEIGHT);
			anim.renderAt(batch, MEAT_MARGIN_HORIZ, y);
			
			String priceString = price + " gp";
			String meatString = monster.getSpecies() + " meat";
			font.draw(batch, monsterSpeciesFormat, meatString, -i * SPECIES_ENTRY_HEIGHT + 10);
			font.draw(batch, monsterSpeciesFormat, priceString, -i * SPECIES_ENTRY_HEIGHT);
		}
		
		for (int i = 0; i < SGlobal.heroes.size(); i += 1) {
			FacesAnimation anim = heroAnims.get(i);
			Chara hero = SGlobal.heroes.getFront(i);
			
			int x = getWidth()/2 + MEAT_MARGIN_HORIZ;
			int y = getHeight() - (MEAT_MARGIN_VERT + INFO_HEIGHT + i * SPECIES_ENTRY_HEIGHT);
			anim.renderAt(batch, x, y);
			
			String string1, string2;
			if (selectingMeat || selectingTarget) {
				string1 = hero.getSpecies();
				Chara selectedMonster = monsters.get(meatIndex);
				MonsterFamily family = hero.getFamily();
				if (family == null) {
					string2 ="";
				} else {
					CharaMDO result = family.getTransformResult(hero, selectedMonster);
					if (result == null) {
						string2 = SConstants.NBSP + "nothing";
					} else {
						string2 = SConstants.NBSP + "to " + result.species;
					}
				}
			} else {
				string1 = hero.getName();
				string2 = hero.getSpecies();
			}
			font.draw(batch, monsterSpeciesFormat, string1, getWidth()/2, -i * SPECIES_ENTRY_HEIGHT + 10);
			font.draw(batch, monsterSpeciesFormat, string2, getWidth()/2, -i * SPECIES_ENTRY_HEIGHT);
		}
		
		if (selectingMeat || selectingTarget) {
			MGlobal.ui.getCursor().renderAt(batch, CURSOR_SPACING,
					getHeight() - (MEAT_MARGIN_VERT + INFO_HEIGHT + meatIndex * SPECIES_ENTRY_HEIGHT) - 4);
		}
		if (selectingTarget) {
			MGlobal.ui.getCursor().renderAt(batch, getWidth()/2 + CURSOR_SPACING,
					getHeight() - (MEAT_MARGIN_VERT + INFO_HEIGHT + charaIndex * SPECIES_ENTRY_HEIGHT) - 4);
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#dispose()
	 */
	@Override
	public void dispose() {
		super.dispose();
		meatsBacker.dispose();
		partyBacker.dispose();
		disposeHeroSprites();
		for (Chara monster : monsters) {
			monster.dispose();
		}
		for (FacesAnimation anim : monsterAnims) {
			anim.dispose();
		}
	}
	
	/**
	 * Called when menu refocuses?
	 */
	protected void resetInfo() {
		info = "Welcome to the gourmet meatmarket.";
	}
	
	/**
	 * Called when the user elects to buy from the shop.
	 * @return					False to keep the menu open
	 */
	protected boolean onBuy() {
		selectingMeat = true;
		meatIndex = 0;
		menu.unfocus();
		
		pushCommandContext(map);
		pushCommandListener(listener);
		
		info = "Which meat?";
		
		return false;
	}
	
	/**
	 * Moves the cursor by the given delta. Infers the proper pointer to move.
	 */
	protected void moveCursor(int delta) {
		if (selectingTarget) {
			charaIndex += delta;
			if (charaIndex < 0) {
				charaIndex = SGlobal.heroes.size() - 1;
			}
			if (charaIndex >= SGlobal.heroes.size()) {
				charaIndex = 0;
			}
		} else {
			meatIndex += delta;
			if (meatIndex < 0) {
				meatIndex = monsters.size() - 1;
			}
			if (meatIndex >= monsters.size()) {
				meatIndex = 0;
			}
		}
	}
	
	/**
	 * Called when player has selected both meat and a target.
	 */
	protected void attemptTransaction() {
		Chara hero = SGlobal.heroes.getFront(charaIndex);
		String oldSpecies = hero.getSpecies();
		hero.eat(monsters.get(meatIndex));
		if (oldSpecies.equals(hero.getSpecies())) {
			info = "Nothing happened";
		} else {
			disposeHeroSprites();
			generateHeroSprites();
			for (Queueable toLoad : heroAnims) {
				MGlobal.assets.loadAsset(toLoad, "new hero sprite");
			}
			info = hero.getName() + " transforms into " + hero.getSpecies();
		}
		SGlobal.heroes.removeGP(mdo.entries[meatIndex].price);
	}
	
	/**
	 * Regens the hero sprites. Necessary after transform.
	 */
	protected void generateHeroSprites() {
		for (Chara hero : SGlobal.heroes.getAll()) {
			FacesAnimation anim = hero.createSprite();
			anim.startMoving();
			heroAnims.add(anim);
			assets.add(anim);
		}
	}
	
	/**
	 * Call before generating sprites.
	 */
	protected void disposeHeroSprites() {
		for (FacesAnimation anim : heroAnims) {
			anim.dispose();
		}
		heroAnims.clear();
	}

}
