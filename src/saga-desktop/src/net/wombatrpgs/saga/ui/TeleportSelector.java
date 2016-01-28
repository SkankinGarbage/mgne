/**
 *  TeleportSelector.java
 *  Created on Jul 12, 2015 4:36:57 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.ui;

import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.utils.Align;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.graphics.ScreenGraphic;
import net.wombatrpgs.mgne.io.CommandListener;
import net.wombatrpgs.mgne.io.CommandMap;
import net.wombatrpgs.mgne.io.command.CMapMenu;
import net.wombatrpgs.mgne.ui.Graphic;
import net.wombatrpgs.mgne.ui.Nineslice;
import net.wombatrpgs.mgne.ui.text.FontHolder;
import net.wombatrpgs.mgne.ui.text.TextFormat;
import net.wombatrpgs.mgneschema.io.data.InputCommand;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.maps.TeleportMemory;

/**
 * Selects a teleport destination. Does not actually perform the teleport.
 */
public class TeleportSelector extends ScreenGraphic implements CommandListener {
	
	protected static final int DISPLAY_X = 0;
	protected static final int DISPLAY_Y = 0;
	protected static final int WIDTH = 148;
	protected static final int HEIGHT = 64;
	protected static final int PROMPT_Y = 48;
	protected static final int LOCATION_Y = 24;
	protected static final int TEXT_HEIGHT = 12;
	
	protected static final String FILENAME_ARROW_UP = "arrow_up.png";
	protected static final String FILENAME_ARROW_DOWN = "arrow_down.png";
	
	protected static final String PROMPT = "To where?";
	
	// ui
	protected Graphic arrowUp, arrowDown;
	protected Nineslice backer;
	protected TextFormat promptFormat, locationFormat;
	
	// selection
	protected CommandMap context;
	protected TeleportSelectionListener listener;
	protected int selection;
	
	/**
	 * Creates a new selector from the global destination. Doesn't start
	 * displaying the selector yet.
	 */
	public TeleportSelector() {
		
		arrowUp = new Graphic(FILENAME_ARROW_UP);
		arrowDown = new Graphic(FILENAME_ARROW_DOWN);
		assets.add(arrowUp);
		assets.add(arrowDown);
		
		backer = new Nineslice(WIDTH, HEIGHT);
		assets.add(backer);
		
		promptFormat = new TextFormat();
		promptFormat.align = Align.center;
		promptFormat.width = WIDTH;
		promptFormat.height = TEXT_HEIGHT;
		promptFormat.x = DISPLAY_X;
		promptFormat.y = DISPLAY_Y + PROMPT_Y;
		
		locationFormat = new TextFormat();
		locationFormat.align = Align.center;
		locationFormat.width = WIDTH;
		locationFormat.height = TEXT_HEIGHT;
		locationFormat.x = DISPLAY_X;
		locationFormat.y = DISPLAY_Y + LOCATION_Y;
		
		context = new CMapMenu();
	}

	/** @see net.wombatrpgs.mgne.graphics.ScreenGraphic#getWidth() */
	@Override public int getWidth() { return WIDTH; }

	/** @see net.wombatrpgs.mgne.graphics.ScreenGraphic#getHeight() */
	@Override public int getHeight() { return HEIGHT; }

	/**
	 * @see net.wombatrpgs.mgne.graphics.ScreenGraphic#coreRender
	 * (com.badlogic.gdx.graphics.g2d.SpriteBatch)
	 */
	@Override
	public void coreRender(SpriteBatch batch) {
		backer.renderAt(batch, DISPLAY_X, DISPLAY_Y);
		
		FontHolder font = MGlobal.ui.getFont();
		font.draw(batch, promptFormat, PROMPT, 0);
		font.draw(batch, locationFormat, getSelectedMemory().displayName, 0);
		
		if (selection > 0) {
			arrowUp.renderAt(batch, DISPLAY_X, DISPLAY_Y + HEIGHT/2);
		} else {
			arrowDown.renderAt(batch, DISPLAY_X, DISPLAY_Y + HEIGHT/2);
		}
		if (selection < SGlobal.teleports.size()-1) {
			arrowDown.renderAt(batch, DISPLAY_X, DISPLAY_Y + HEIGHT/2 - arrowDown.getHeight());
		} else {
			arrowUp.renderAt(batch, DISPLAY_X, DISPLAY_Y + HEIGHT/2 - arrowUp.getHeight());
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.io.CommandListener#onCommand
	 * (net.wombatrpgs.mgneschema.io.data.InputCommand)
	 */
	@Override
	public boolean onCommand(InputCommand command) {
		switch (command) {
		case MOVE_DOWN:			moveCursor(1);			return true;
		case MOVE_UP:			moveCursor(-1);			return true;
		case UI_CONFIRM:		onConfirm();			return true;
		case UI_CANCEL:			onCancel();				return true;
		default:										return true;
		}
	}
	
	/**
	 * Displays this selector and waits for a response.
	 * @param listener
	 */
	public void selectLocation(TeleportSelectionListener listener) {
		this.listener = listener;
		this.parent = MGlobal.screens.peek();
		parent.addChild(this);
		parent.pushCommandContext(context);
		parent.pushCommandListener(this);
	}
	
	/**
	 * Gets the selected memory.
	 * @return					The selected memory
	 */
	protected TeleportMemory getSelectedMemory() {
		return SGlobal.teleports.get(selection);
	}
	
	/**
	 * Moves the selection by a given amount. Handles wrapping.
	 * @param	delta			The amount to move the cursor by
	 */
	protected void moveCursor(int delta) {
		selection += delta;
		if (selection < 0) {
			selection = SGlobal.teleports.size()-1;
		}
		if (selection > SGlobal.teleports.size()-1) {
			selection = 0;
		}
	}
	
	/**
	 * Called when user opts to teleport.
	 */
	protected void onConfirm() {
		finish();
		listener.onFinish(getSelectedMemory());
	}
	
	/**
	 * Called when the user opts not to teleport.
	 */
	protected void onCancel() {
		finish();
		listener.onFinish(null);
	}
	
	/**
	 * Dismisses the selector.
	 */
	protected void finish() {
		parent.removeCommandListener(this);
		parent.removeCommandContext(context);
		parent.removeChild(this);
		backer.dispose();
	}
	
	/**
	 * Callback for teleport selector.
	 */
	public abstract class TeleportSelectionListener {
		
		/**
		 * Called by the teleport selector when user is done.
		 * @param	memory			The selected memory, or null for canceled
		 */
		public abstract void onFinish(TeleportMemory memory);
	}

}
