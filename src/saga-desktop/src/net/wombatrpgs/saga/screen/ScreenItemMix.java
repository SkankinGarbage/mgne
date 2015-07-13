/**
 *  ScreenItemMix.java
 *  Created on Jul 11, 2015 6:55:51 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.screen;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import com.badlogic.gdx.graphics.g2d.BitmapFont.HAlignment;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.io.CommandMap;
import net.wombatrpgs.mgne.io.command.CMapMenu;
import net.wombatrpgs.mgne.ui.Graphic;
import net.wombatrpgs.mgne.ui.Nineslice;
import net.wombatrpgs.mgne.ui.text.FontHolder;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.mgneschema.io.data.InputCommand;
import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.items.Collectable;
import net.wombatrpgs.saga.rpg.items.CollectableSet;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.saga.ui.CollectionSelector;
import net.wombatrpgs.saga.ui.DescriptionBox;
import net.wombatrpgs.sagaschema.rpg.abil.CollectableMDO;
import net.wombatrpgs.sagaschema.rpg.abil.CombatItemMDO;
import net.wombatrpgs.sagaschema.rpg.mix.ItemMixMDO;
import net.wombatrpgs.sagaschema.rpg.mix.ItemMixSetMDO;
import net.wombatrpgs.sagaschema.rpg.mix.data.MixEntryMDO;

/**
 * Displays the crafting interface for a set of collectable combinations.
 */
public class ScreenItemMix extends SagaScreen {
	
	protected static int WIDTH = 320;
	protected static int INSTRUCTIONS_HEIGHT = 24;
	protected static int CONSTRUCTION_WIDTH = 320;
	protected static int CONSTRUCTION_HEIGHT = 140;
	protected static int DESCRIPTION_HEIGHT = 36;
	protected static int PADDING = 3;
	protected static int TEXT_WIDTH = 240;
	protected static int TEXT_HEIGHT = 16;
	protected static int INGREDIENT_WIDTH1 = 76;
	protected static int INGREDIENT_WIDTH2 = 168;
	protected static int SELECTOR_WIDTH = 88;
	protected static int MARGIN = 26;
	
	protected ItemMixSetMDO mdo;
	
	// state
	protected CollectableSet collection;
	protected Set<Collectable> sourceItems;
	protected List<ItemMixMDO> mixMDOs;
	protected String instructions;
	protected int selection;
	protected boolean done;
	
	// ui
	protected CommandMap context;
	protected CollectionSelector selector;
	protected Nineslice instructionsBG, constructionBG, collectionBG;
	protected DescriptionBox descriptionBox;
	protected TextFormat instructionsFormat, constructionFormat;
	
	/**
	 * Creates a new screen to mix a given combination of items.
	 * @param	mdo				The mdo containing the set of items to display
	 */
	public ScreenItemMix(ItemMixSetMDO mdo) {
		this.mdo = mdo;
		
		context = new CMapMenu();
		
		mixMDOs = new ArrayList<ItemMixMDO>();
		for (String mixMDOKey : mdo.combinations) {
			mixMDOs.add(MGlobal.data.getEntryFor(mixMDOKey, ItemMixMDO.class));
		}
		
		sourceItems = new HashSet<Collectable>();
		for (ItemMixMDO mixMDO : mixMDOs) {
			for (MixEntryMDO ingredientMDO : mixMDO.ingredients) {
				sourceItems.add(new Collectable(ingredientMDO.item));
			}
		}
		collection = new CollectableSet();
		regenerateCollection();
		
		instructionsBG = new Nineslice(WIDTH, INSTRUCTIONS_HEIGHT);
		assets.add(instructionsBG);
		
		constructionBG = new Nineslice(CONSTRUCTION_WIDTH, CONSTRUCTION_HEIGHT);
		assets.add(constructionBG);
		
		int height = INSTRUCTIONS_HEIGHT + CONSTRUCTION_HEIGHT + DESCRIPTION_HEIGHT;
		height -= constructionBG.getBorderHeight() * 2;
//		int displayY = (getHeight() - height) / 2;
		int displayY = (getHeight() - height);
		int displayX = (getWidth() - WIDTH) / 2;
		
		collectionBG = new Nineslice(SELECTOR_WIDTH, getHeight() - height + constructionBG.getBorderHeight());
		assets.add(collectionBG);
		
		descriptionBox = new DescriptionBox(WIDTH, DESCRIPTION_HEIGHT);
		assets.add(descriptionBox);
		descriptionBox.setX(displayX);
		descriptionBox.setY(displayY);
		descriptionBox.setPadding(PADDING);
		displayY += descriptionBox.getHeight() - constructionBG.getBorderHeight();
		displayY += CONSTRUCTION_HEIGHT;
		
		selector = new CollectionSelector(collection, 4, SELECTOR_WIDTH - PADDING*6, PADDING, true);
		assets.add(selector);
		selector.setX(displayX + collectionBG.getBorderWidth() + PADDING);
		selector.setY(0);
		
		instructionsFormat = new TextFormat();
		instructionsFormat.x = displayX + PADDING*3;
		instructionsFormat.y = displayY + INSTRUCTIONS_HEIGHT - PADDING*4 - 1;
		instructionsFormat.align = HAlignment.LEFT;
		instructionsFormat.width = TEXT_WIDTH;
		instructionsFormat.height = TEXT_HEIGHT;
		
		constructionFormat = new TextFormat();
		constructionFormat.x = displayX + MARGIN;
		constructionFormat.y = displayY - PADDING*4;
		constructionFormat.align = HAlignment.LEFT;
		constructionFormat.width = TEXT_WIDTH;
		constructionFormat.height = TEXT_HEIGHT;
		
		instructions = "What do you want to make?";
		
		refreshDescription();
	}
	
	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#render
	 * (com.badlogic.gdx.graphics.g2d.SpriteBatch)
	 */
	@Override
	public void render(SpriteBatch batch) {
		super.render(batch);
		
		int height = INSTRUCTIONS_HEIGHT + CONSTRUCTION_HEIGHT + DESCRIPTION_HEIGHT;
		height -= constructionBG.getBorderHeight() * 2;
//		int displayY = (getHeight() - height) / 2;
		int displayY = (getHeight() - height);
		int displayX = (getWidth() - WIDTH) / 2;
		
		collectionBG.renderAt(batch, 0, 0);
		selector.render(batch);
		
		descriptionBox.render(batch);
		displayY += descriptionBox.getHeight() - constructionBG.getBorderHeight();
		
		constructionBG.renderAt(batch, displayX, displayY);
		displayY += CONSTRUCTION_HEIGHT - constructionBG.getBorderHeight();
		
		instructionsBG.renderAt(batch, displayX, displayY);
		
		FontHolder font = MGlobal.ui.getFont();
		font.draw(batch, instructionsFormat, instructions, 0);
		
		for (int i = 0; i < mixMDOs.size(); i += 1) {
			int offsetY = (int) (-i * (font.getLineHeight() + PADDING));
			ItemMixMDO mdo = mixMDOs.get(i);
			if (mdo.ingredients.length > 0) {
				MixEntryMDO ingredient = mdo.ingredients[0];
				CollectableMDO collectable = MGlobal.data.getEntryFor(ingredient.item, CollectableMDO.class);
				String text = String.valueOf(mdo.ingredients[0].quantity);
				text += " " + collectable.displayName;
				text = MGlobal.charConverter.convert(text);
				font.draw(batch, constructionFormat, text, 0, offsetY);
			}
			if (mdo.ingredients.length > 1) {
				MixEntryMDO ingredient = mdo.ingredients[1];
				CollectableMDO collectable = MGlobal.data.getEntryFor(ingredient.item, CollectableMDO.class);
				String text = "+ " + String.valueOf(mdo.ingredients[1].quantity);
				text += " " + collectable.displayName;
				text = MGlobal.charConverter.convert(text);
				font.draw(batch, constructionFormat, text, INGREDIENT_WIDTH1, offsetY);
			}
			CombatItemMDO result = MGlobal.data.getEntryFor(mdo.result, CombatItemMDO.class);
			String text  = "= " + result.abilityName;
			text = MGlobal.charConverter.convert(text);
			font.draw(batch, constructionFormat, text, INGREDIENT_WIDTH2, offsetY);
			
			if (selection == i) {
				Graphic cursor = MGlobal.ui.getCursor();
				cursor.renderAt(batch, PADDING*2, offsetY + constructionFormat.y - cursor.getHeight());
			}
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onFocusLost()
	 */
	@Override
	public void onFocusLost() {
		super.onFocusLost();
		removeCommandContext(context);
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.Screen#onFocusGained()
	 */
	@Override
	public void onFocusGained() {
		super.onFocusGained();
		pushCommandContext(context);
	}
	
	/**
	 * @see net.wombatrpgs.saga.screen.SagaScreen#onCommand
	 * (net.wombatrpgs.mgneschema.io.data.InputCommand)
	 */
	@Override
	public boolean onCommand(InputCommand command) {
		boolean superResult = super.onCommand(command);
		if (superResult) return superResult;
		switch(command) {
		case MOVE_DOWN:			moveCursor(1);			return true;
		case MOVE_UP:			moveCursor(-1);			return true;
		case UI_CONFIRM:		confirm();				return true;
		case UI_CANCEL:			cancel();				return true;
		default:										return false;
		}
	}

	/**
	 * Checks if the user is done with this screen.
	 * @return					True if user is done, false otherwise
	 */
	public boolean isDone() {
		return done;
	}

	/**
	 * Regenerates the internal collection of available items.
	 */
	protected void regenerateCollection() {
		for (Collectable collectable : sourceItems) {
			collection.copyCollectableValue(SGlobal.heroes.getCollection(), collectable);
		}
	}
	
	/**
	 * Gets the description box describing the right item.
	 */
	protected void refreshDescription() {
		CombatItemMDO selectedItem = MGlobal.data.getEntryFor(
				mixMDOs.get(selection).result, CombatItemMDO.class);
		descriptionBox.describe(selectedItem);
	}
	
	/**
	 * Moves the selection cursor by the given offset.
	 * @param	delta			The amount to move the cursor by, neg for up
	 */
	protected void moveCursor(int delta) {
		selection += delta;
		if (selection < 0) {
			selection = mixMDOs.size() - 1;
		}
		if (selection >= mixMDOs.size()) {
			selection = 0;
		}
		refreshDescription();
	}
	
	/**
	 * Called when the user tries to make a combination.
	 */
	protected void confirm() {
		ItemMixMDO mix = mixMDOs.get(selection);
		Collectable missing = null;
		for (MixEntryMDO entry : mix.ingredients) {
			if (collection.getQuantity(entry.item) < entry.quantity) {
				missing = new Collectable(entry.item);
				break;
			}
		}
		
		if (missing != null) {
			instructions = "Not enough " + missing.getName() + ".";
			return;
		}
		
		if (SGlobal.heroes.getInventory().isFull()) {
			instructions = "No room.";
			return;
		}
		
		for (MixEntryMDO entry : mix.ingredients) {
			SGlobal.heroes.getCollection().subtractCollectable(entry.item, entry.quantity);
		}
		regenerateCollection();
		MGlobal.audio.playSFX(SConstants.SFX_GET);
		instructions = "Here you go.";
		SGlobal.heroes.addItem(new CombatItem(mix.result));
	}
	
	/**
	 * Closes this screen.
	 */
	protected void cancel() {
		MGlobal.screens.pop();
		instructionsBG.dispose();
		constructionBG.dispose();
		collectionBG.dispose();
		done = true;
	}

}
