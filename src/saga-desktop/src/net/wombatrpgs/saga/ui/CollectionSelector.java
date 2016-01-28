/**
 *  AbilSelector.java
 *  Created on Apr 12, 2014 2:47:33 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.ui;

import java.util.Map;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.utils.Align;

import net.wombatrpgs.mgne.core.MAssets;
import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.graphics.ScreenGraphic;
import net.wombatrpgs.mgne.io.CommandListener;
import net.wombatrpgs.mgne.io.CommandMap;
import net.wombatrpgs.mgne.io.command.CMapRaw;
import net.wombatrpgs.mgne.screen.Screen;
import net.wombatrpgs.mgne.ui.Graphic;
import net.wombatrpgs.mgne.ui.text.FontHolder;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.mgneschema.io.data.InputCommand;
import net.wombatrpgs.saga.rpg.items.CollectableSet;

/**
 * Allows the user to select an item from a collection.
 */
public class CollectionSelector extends ScreenGraphic implements CommandListener {
	
	protected static final int INDENT_SIZE = 4;
	
	protected CollectableSet collection;
	protected CommandMap context;
	
	// layout
	protected Screen parent;
	protected TextFormat nameFormat, quantityFormat;
	protected int count;
	protected int width, height;
	protected int padding;
	
	// cursor
	protected SlotListener listener, selectListener;
	protected boolean cursorOn;
	protected boolean cancellable;
	protected float cursorX, cursorY;
	protected int selected;
	
	// cursor indent
	protected boolean indentOn;
	protected float indentX, indentY;
	
	protected boolean displayIfEmpty;
	
	/**
	 * Creates a new selector for a given inventory.
	 * @param	collection		The collection whose contents to display
	 * @param	count			The number of collectables to display at once
	 * @param	width			The width of the selector (in virt px)
	 * @param	padding			The vertical padding between items (in virt px)
	 * @param	displayIfEmpty	True to display even a blank collection
	 */
	public CollectionSelector(CollectableSet collection, int count, int width,
			int padding, boolean displayIfEmpty) {
		this.collection = collection;
		this.width = width;
		this.padding = padding;
		this.displayIfEmpty = displayIfEmpty;
		
		FontHolder font = MGlobal.ui.getFont();
		height = (int) (count * font.getLineHeight());
		height += padding * (count-1);
		
		nameFormat = new TextFormat();
		nameFormat.align = Align.left;
		nameFormat.width = width;
		nameFormat.height = 240;
		
		quantityFormat = new TextFormat();
		quantityFormat.align = Align.right;
		quantityFormat.width = 16;
		quantityFormat.height = 240;
		
		context = new CMapRaw();
	}

	/** @see net.wombatrpgs.mgne.graphics.ScreenGraphic#getWidth() */
	@Override public int getWidth() { return width; }

	/** @see net.wombatrpgs.mgne.graphics.ScreenGraphic#getHeight() */
	@Override public int getHeight() { return height; }

	/**
	 * @see net.wombatrpgs.mgne.graphics.ScreenGraphic#coreRender
	 * (com.badlogic.gdx.graphics.g2d.SpriteBatch)
	 */
	@Override
	public void coreRender(SpriteBatch batch) {
		FontHolder font = MGlobal.ui.getFont();
		Map<String, Integer> pairs = collection.toNameQuantityPairs();
		if (!displayIfEmpty && pairs.size() == 0) return;
		int i = 0;
		for (String name : pairs.keySet()) {
			int offY = (int) (-i * (font.getLineHeight() + padding));
			font.draw(batch, nameFormat, name, offY);
			String quantity = pairs.get(name).toString();
			font.draw(batch, quantityFormat, quantity, offY);
			i += 1;
		}
		
		Graphic cursor = MGlobal.ui.getCursor();
		if (indentOn) {
			cursor.renderAt(batch, indentX, indentY);
		}
		if (cursorOn) {
			cursor.renderAt(batch, cursorX, cursorY);
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.io.CommandListener#onCommand
	 * (net.wombatrpgs.mgneschema.io.data.InputCommand)
	 */
	@Override
	public boolean onCommand(InputCommand command) {
		switch (command) {
		case RAW_UP:		moveCursor(-1);		return true;
		case RAW_DOWN:		moveCursor(1);		return true;
		case RAW_A:			confirm();			return true;
		case RAW_B:			cancel();			return true;
		case RAW_START:		cancel();			return true;
		case RAW_SELECT:	select();			return true;
		default:								return true;
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.screen.ScreenObject#postProcessing
	 * (MAssets, int)
	 */
	@Override
	public void postProcessing(MAssets manager, int pass) {
		super.postProcessing(manager, pass);
		FontHolder font = MGlobal.ui.getFont();
		nameFormat.x = (int) x;
		nameFormat.y = (int) (y + height + font.getLineHeight());
		quantityFormat.x = (int) (x + width - quantityFormat.width );
		quantityFormat.y = (int) (y + height + font.getLineHeight());
	}
	
	/**
	 * Sets the selected slot to be whatever and updates cursor position.
	 * @param	slot			The new slot to select
	 * @return					The old selected slot
	 */
	public int setSelected(int slot) {
		int old = selected;
		selected = slot;
		updateCursor();
		return old;
	}
	
	/**
	 * Marks the selected cursor position by indenting a copy of the cursor.
	 */
	public void setIndent() {
		indentOn = true;
		indentX = cursorX + INDENT_SIZE;
		indentY = cursorY;
	}
	
	/**
	 * Removes the indented cursor copy.
	 */
	public void clearIndent() {
		indentOn = false;
	}
	
	/**
	 * Waits for the user to select a slot then returns to the listener.
	 * @param	listener		The callback for selection
	 * @param	canCancel		True if the user can cancel to select nobody
	 */
	public void awaitSelection(SlotListener listener, boolean canCancel) {
		this.listener = listener;
		this.cancellable = canCancel;
		focus();
		cursorOn = true;
		
		selected = 0;
		updateCursor();
	}
	
	/**
	 * Also listens for when the user presses select on an item for some reason.
	 * @param	listener		The listener to call when a slot is selected
	 */
	public void attachSelectListener(SlotListener listener) {
		this.selectListener = listener;
	}
	
	/**
	 * Stops this menu from receiving input. It still displays on the screen.
	 */
	public void unfocus() {
		cursorOn = false;
		parent.removeCommandListener(this);
		parent.removeCommandContext(context);
	}
	
	/**
	 * Resumes the menu for input reception. Should already be on screen.
	 */
	protected void focus() {
		clearIndent();
		parent = MGlobal.screens.peek();
		parent.pushCommandContext(context);
		parent.pushCommandListener(this);
	}
	
	/**
	 * Called when user cancels the selection process.
	 */
	protected void cancel() {
		if (cancellable) {
			handleListener(listener.onSelection(-1));
		}
	}
	
	/**
	 * Called when the user confirms a character selection.
	 */
	protected void confirm() {
		handleListener(listener.onSelection(selected));
	}
	
	/**
	 * Called when the weird user presses select on an item?
	 */
	protected void select() {
		handleListener(selectListener.onSelection(selected));
	}
	
	/**
	 * Moves the cursor by the delta.
	 * @param	delta			The delta to move the cursor by
	 */
	protected void moveCursor(int delta) {
		selected += delta;
		if (selected < 0) selected = count - 1;
		if (selected >= count) selected = 0;
		updateCursor();
	}
	
	/**
	 * Updates the cursor's position.
	 */
	protected void updateCursor() {
		Graphic cursor = MGlobal.ui.getCursor();
		FontHolder font = MGlobal.ui.getFont();
		cursorX = x - cursor.getWidth() - 3;
		cursorY = y + height - (selected * (font.getLineHeight() + padding) + cursor.getHeight()/2);
	}
	
	/**
	 * Handles the result from the callback function.
	 * @param	result			True to unfocus the listener
	 */
	protected void handleListener(boolean result) {
		if (result) {
			unfocus();
		}
	}

}
