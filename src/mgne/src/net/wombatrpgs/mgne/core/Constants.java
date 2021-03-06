/**
 *  Constants.java
 *  Created on Nov 25, 2012 12:39:01 AM for project rainfall-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgne.core;

import java.util.ArrayList;
import java.util.List;

import net.wombatrpgs.mgneschema.settings.GameSpeedMDO;
import net.wombatrpgs.mgneschema.settings.UISettingsMDO;
import net.wombatrpgs.mgneschema.settings.WindowSettingsMDO;
import net.wombatrpgs.mgns.core.MainSchema;

/**
 * Access to a bunch of magic numbers that are needed in a bunch of different
 * places. The idea is to only load up the schema once. Also holds other non-
 * database defined constants, or things that should be database constants
 * eventually.
 */
public class Constants {
	
	/** Where the game start info is */
	public static final String CONFIG_FILE = "config.cfg";
	
	/** Directories where resources are stored (prefix string) */
	public static final String RESOURCE_DIR = "res/";
	public static final String SPRITES_DIR = RESOURCE_DIR + "sprites/";
	public static final String MAPS_DIR = RESOURCE_DIR + "maps/";
	public static final String GIBS_DIR = SPRITES_DIR;
	public static final String FONTS_DIR = RESOURCE_DIR + "fonts/";
	public static final String UI_DIR = RESOURCE_DIR + "ui/";
	public static final String SCENES_DIR = RESOURCE_DIR + "scenes/";
	public static final String DATA_DIR = RESOURCE_DIR + "data/";
	public static final String AUDIO_DIR = RESOURCE_DIR + "audio/";
	public static final String SHADERS_DIR = RESOURCE_DIR + "shaders/";
	public static final String TILES_DIR = RESOURCE_DIR + "tiles/";
	public static final String TEXTURES_DIR = RESOURCE_DIR + "textures/";
	public static final String ITEMS_DIR = RESOURCE_DIR + "items/";
	
	/** Save file information */
	public static final String SAVES_DIR = "savedata/";
	public static final String SAVES_PREFIX = "save";
	public static final String SAVES_SUFFIX = ".sav";
	
	/** Keys for the unique MDOs in the database */
	public static final String KEY_WINDOW = "window_data";
	public static final String KEY_TITLE = "default_title";
	public static final String KEY_INTRO = "default_intro";
	public static final String KEY_DEATH = "default_death";
	public static final String KEY_INPUT = "default_input";
	
	/** Constants for Tiled terrain */
	public static final String PROPERTY_PASSABLE = "o";
	public static final String PROPERTY_IMPASSABLE = "x";
	public static final String PROPERTY_Z = "z";
	public static final String PROPERTY_FIRST_GID = "firstgid";
	public static final String PROPERTY_COUNTER = "counter";
	public static final String PROPERTY_FORCE = "force";
	
	/** Strings could be null or else this */
	public static final String NULL_MDO = "None";
	
	/** Default supported argument keys */
	public static final String ARG_FULLSCREEN = "fullscreen";
	public static final String ARG_MAP = "map";
	public static final String ARG_X = "x";
	public static final String ARG_Y = "y";
	public static final String ARG_CONTROL_A = "a";
	public static final String ARG_CONTROL_B = "b";
	public static final String ARG_CONTROL_START = "start";
	public static final String ARG_CONTROL_SELECT = "select";
	public static final String ARG_CONTROL_LEFT = "left";
	public static final String ARG_CONTROL_RIGHT = "right";
	public static final String ARG_CONTROL_UP = "up";
	public static final String ARG_CONTROL_DOWN = "down";
	public static final String ARG_TEXT_SPEED = "textspeed";
	
	public static final List<Class<? extends MainSchema>> PRELOAD_SCHEMA;
	static {
		PRELOAD_SCHEMA = new ArrayList<Class< ? extends MainSchema>>();
		PRELOAD_SCHEMA.add(WindowSettingsMDO.class);
		PRELOAD_SCHEMA.add(GameSpeedMDO.class);
		PRELOAD_SCHEMA.add(UISettingsMDO.class);
	}
	
	private int rate;
	private float delay;
	
	/**
	 * Reads the database for a bunch of constants.
	 */
	public Constants() {
		// load everything up~
		rate = MGlobal.data.getEntryFor("game_speed", GameSpeedMDO.class).framerate;
		delay = MGlobal.data.getEntryFor("game_speed", GameSpeedMDO.class).delay;
	}
	
	/** @return The target game framerate */
	public int getRate() { return rate; }
	
	/** @return The base delay between steps */
	public float getDelay() { return delay; }

}
