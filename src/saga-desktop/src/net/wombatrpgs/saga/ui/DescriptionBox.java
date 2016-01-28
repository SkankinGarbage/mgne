/**
 *  DescriptionBox.java
 *  Created on Jan 4, 2015 9:38:25 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.ui;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.utils.Align;

import net.wombatrpgs.mgne.core.MAssets;
import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.graphics.ScreenGraphic;
import net.wombatrpgs.mgne.ui.Nineslice;
import net.wombatrpgs.mgne.ui.text.FontHolder;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.sagaschema.rpg.abil.CombatItemMDO;

/**
 * Specialized SaGa description box used to display item/chara descriptions on
 * menus in noob mode.
 */
public class DescriptionBox extends ScreenGraphic {
	
	protected static int MAX_CHARS = 80;
	
	protected Nineslice bg;
	protected TextFormat format;
	protected String text;
	protected int width, height, padding;
	
	/**
	 * Creates a new description box with no text and the given dimensions.
	 * @param	width			The width of this description box, in pixels
	 * @param	height			The height of this description box, in pixels
	 */
	public DescriptionBox(int width, int height) {
		this.width = width;
		this.height = height;
		
		bg = new Nineslice(width, height);
		assets.add(bg);
		
		format = new TextFormat();
		format.align = Align.left;
		format.width = width - bg.getBorderWidth()*2;
		format.height = height - bg.getBorderHeight()*2;
		
		text = "";
	}

	/**  @see net.wombatrpgs.mgne.graphics.ScreenGraphic#getWidth() */
	@Override public int getWidth() { return width; }

	/** @see net.wombatrpgs.mgne.graphics.ScreenGraphic#getHeight() */
	@Override public int getHeight() { return height; }
	
	/** @param padding The new horiz offset for the text */
	public void setPadding(int padding) {
		this.padding = padding;
		format.width = width - bg.getBorderWidth()*2 - padding*2;
	}

	/**
	 * @see net.wombatrpgs.mgne.graphics.ScreenGraphic#coreRender
	 * (com.badlogic.gdx.graphics.g2d.SpriteBatch)
	 */
	@Override
	public void coreRender(SpriteBatch batch) {
		bg.renderAt(batch, x, y);
		FontHolder font = MGlobal.ui.getFont();
		font.draw(batch, format, text, 0);
	}
	
	/**
	 * @see net.wombatrpgs.mgne.core.AssetQueuer#postProcessing
	 * (net.wombatrpgs.mgne.core.MAssets, int)
	 */
	@Override public void postProcessing(MAssets manager, int pass) {
		super.postProcessing(manager, pass);
		format.x = (int) (x + bg.getBorderWidth() + padding);
		format.y = (int) (y + height - bg.getBorderHeight());
	}

	/**
	 * Sets the text to describe an item.
	 * @param	item			The item to describe, or null for no text
	 */
	public void describe(CombatItem item) {
		if (item == null) {
			text = "";
		} else {
			text = item.getDescription();
		}
	}
	
	/**
	 * Sets the text to describe an item described by an MDO.
	 * @param	itemMDO			The data of the item to display
	 */
	public void describe(CombatItemMDO itemMDO) {
		if (itemMDO == null) {
			text = "";
		} else {
			text = MGlobal.charConverter.convert(itemMDO.itemDescription);
		}
	}

}
