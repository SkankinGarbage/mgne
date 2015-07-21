/**
 *  Hero.java
 *  Created on Nov 25, 2012 8:33:22 PM for project rainfall-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.maps.events;

import java.util.ArrayList;
import java.util.List;

import net.wombatrpgs.mgne.core.Constants;
import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.mgne.core.interfaces.FinishListener;
import net.wombatrpgs.mgne.graphics.FacesAnimationFactory;
import net.wombatrpgs.mgne.io.CommandListener;
import net.wombatrpgs.mgne.io.Keymap.KeyState;
import net.wombatrpgs.mgneschema.io.data.InputButton;
import net.wombatrpgs.mgneschema.io.data.InputCommand;
import net.wombatrpgs.mgneschema.maps.EventMDO;
import net.wombatrpgs.mgneschema.maps.data.OrthoDir;

/**
 * The physical representation of the player on the world map.
 */
public class Avatar extends MapEvent implements CommandListener {
	
	protected static final String HERO_DEFAULT = "event_hero";
	
	protected List<FinishListener> stepListeners;
	protected String parentName;
	protected OrthoDir forceDir;
	protected boolean paused;

	/**
	 * For real hero constructor. Looks up the avatar in the database and
	 * uses it to set up a map event. Takes an argument so that kryo doesn't try
	 * to call this.
	 */
	public Avatar() {
		super(MGlobal.data.getEntryFor(HERO_DEFAULT, EventMDO.class));
		stepListeners = new ArrayList<FinishListener>();
		addStepTracker();
		
		stepListeners.add(new FinishListener() {
			@Override public void onFinish() {
				updateForceDir();
			}
		});
	}
	
	/**
	 * Creates a new avatar from a stored snapshot.
	 * @param	memory			The memory to restore from
	 */
	public Avatar(AvatarMemory memory) {
		this();
		setTileX(memory.tileX);
		setTileY(memory.tileY);
		appearance = FacesAnimationFactory.create(memory.animKey);
		appearance.startMoving();
		assets.add(appearance);
		setFacing(memory.dir);
	}

	/**
	 * @see net.wombatrpgs.mgne.maps.MapThing#reset()
	 */
	@Override
	public void reset() {
		// oh hell no we ain't dyin
		// ^^ p sure that's been around since the Blockbound days
	}

	/**
	 * @see net.wombatrpgs.mgne.maps.events.MapEvent#getName()
	 */
	@Override
	public String getName() {
		return "hero";
	}

	/**
	 * @see net.wombatrpgs.mgne.maps.events.MapEvent#update(float)
	 */
	@Override
	public void update(float elapsed) {
		float oldVX = vx;
		float oldVY = vy;
		if (forceDir != null) {
			move(forceDir);
			OrthoDir heldDir = null;
			if (MGlobal.keymap.getButtonState(InputButton.DOWN) == KeyState.DOWN) {
				heldDir = OrthoDir.SOUTH;
			} else if (MGlobal.keymap.getButtonState(InputButton.LEFT) == KeyState.DOWN) {
				heldDir = OrthoDir.WEST;
			} else if (MGlobal.keymap.getButtonState(InputButton.RIGHT) == KeyState.DOWN) {
				heldDir = OrthoDir.EAST;
			} else if (MGlobal.keymap.getButtonState(InputButton.UP) == KeyState.DOWN) {
				heldDir = OrthoDir.NORTH;
			}
			if (heldDir != null) {
				setFacing(heldDir);
			}
			float mult;
			if (forceDir == heldDir) {
				mult = 2f;
			} else if (OrthoDir.getOpposite(forceDir) == heldDir) {
				mult = .5f;
			} else {
				mult = 1f;
			}
			vx *= mult;
			vy *= mult;
		}
		super.update(elapsed);
		if (forceDir != null) {
			updateForceDir();
			if (forceDir != null) {
				vx = oldVX;
				vy = oldVY;
			}
		}
	}

	/**
	 * @see net.wombatrpgs.mgne.io.CommandListener#onCommand
	 * (net.wombatrpgs.mgneschema.io.data.InputCommand)
	 */
	@Override
	public boolean onCommand(InputCommand command) {
		if (!tracking && !paused && forceDir == null) {
			switch (command) {
			case MOVE_LEFT:			move(OrthoDir.WEST);	break;
			case MOVE_UP:			move(OrthoDir.NORTH);	break;
			case MOVE_RIGHT:		move(OrthoDir.EAST);	break;
			case MOVE_DOWN:			move(OrthoDir.SOUTH);	break;
			case WORLD_INTERACT:	interact();				break;
			default:				return false;
			}
		}
		return true;
	}
	
	/**
	 * Sets the paused state. Paused heroes can't move or interact.
	 * @param	paused			True to pause, false to unpause
	 */
	public void pause(boolean paused) {
		this.paused = paused;
	}
	
	/**
	 * Adds a callback for when the hero finishes taking a step. Make sure to
	 * remove it when no longer necessary.
	 * @param	listener		The listener to call
	 */
	public void addStepListener(FinishListener listener) {
		stepListeners.add(listener);
	}
	
	/**
	 * Removes a callback for hero stepping.
	 * @param	listener		The listener to remove
	 */
	public void removeStepListener(FinishListener listener) {
		if (!stepListeners.contains(listener)) {
			MGlobal.reporter.warn("Tried to remove non-added stepper");
		}
		stepListeners.remove(listener);
	}
	
	/**
	 * Called when this avatar is loaded from memory.
	 */
	public void onUnloaded() {
		this.vx = 0;
		this.vy = 0;
		this.tracking = false;
	}

	/**
	 * Moves in a certain dir on the map?
	 * @param	dir				The direction to move
	 */
	protected void move(OrthoDir dir) {
		int targetX = (int) (getTileX() + dir.getVector().x);
		int targetY = (int) (getTileY() + dir.getVector().y);
		if (attemptStep(dir)) {
			parent.onTurn();
		} else {
			for (MapEvent event : parent.getEventsAt(targetX, targetY)) {
				if (event != this && !event.isPassable()) {
					event.onCollide(this);
				}
			}
		}
	}
	
	/**
	 * Interact with whatever we're standing on or facing.
	 * @see net.wombatrpgs.mgne.maps.events.MapEvent#onInteract()
	 */
	protected void interact() {
		for (MapEvent event : parent.getEventsAt(getTileX(), getTileY())) {
			if (event == this) continue;
			if (!event.isPassable()) continue;
			if (event.isHidden()) continue;
			if (event.onInteract()) return;
		}
		OrthoDir facing = getFacing();
		int tileX = (int) (getTileX() + facing.getVector().x);
		int tileY = (int) (getTileY() + facing.getVector().y);
		for (MapEvent event : parent.getEventsAt(tileX, tileY)) {
			if (event.isPassable()) continue;
			if (event.onInteract()) return;
		}
		if (parent.tileHasProperty(tileX, tileY, Constants.PROPERTY_COUNTER)) {
			tileX += (int) facing.getVector().x;
			tileY += (int) facing.getVector().y;
			for (MapEvent event : parent.getEventsAt(tileX, tileY)) {
				if (event.isPassable()) continue;
				if (event.onInteract()) return;
			}
		}
	}
	
	/**
	 * Adds the step tracker for when steps finish.
	 */
	protected void addStepTracker() {
		addTrackingListener(new FinishListener() {
			@Override public void onFinish() {
				for (FinishListener listener : stepListeners) {
					listener.onFinish();
				}
				addStepTracker();
			}
		});
	}
	
	/**
	 * Checks the direction that this tile is being forced towards
	 */
	protected void updateForceDir() {
		forceDir = null;
		if (parent.tileHasProperty(tileX, tileY, Constants.PROPERTY_FORCE)) {
			String dirString = parent.getTileProperty(tileX, tileY, Constants.PROPERTY_FORCE);
			if (dirString != null) {
				forceDir = OrthoDir.valueOf(dirString);
				halt();
			}
		}
	}
}
