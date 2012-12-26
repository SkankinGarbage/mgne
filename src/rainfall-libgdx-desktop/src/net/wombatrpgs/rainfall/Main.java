package net.wombatrpgs.rainfall;

import net.wombatrpgs.mgne.data.Database;
import net.wombatrpgs.mgne.data.DirectoryDataLoader;
import net.wombatrpgs.mgne.global.DebugReporter;
import net.wombatrpgs.mgne.global.SimpleFileLoader;
import net.wombatrpgs.rainfall.core.Constants;
import net.wombatrpgs.rainfall.core.RGlobal;
import net.wombatrpgs.rainfall.core.ScreenStack;
import net.wombatrpgs.rainfall.io.DefaultKeymap;
import net.wombatrpgs.rainfallschema.settings.WindowDataMDO;

import com.badlogic.gdx.assets.AssetManager;
import com.badlogic.gdx.backends.lwjgl.LwjglApplication;
import com.badlogic.gdx.backends.lwjgl.LwjglApplicationConfiguration;
import com.fasterxml.jackson.databind.ObjectMapper;

/**
 * Autogenerated by project setup.
 */
public class Main {
	
	/**
	 * Main launcher. Autogenerated.
	 * @param 	args		Unusused
	 */
	public static void main(String[] args) {
		
		// TODO: set up a loading bar or something
		RGlobal.reporter = new DebugReporter();
		RGlobal.fileLoader = new SimpleFileLoader();
		RGlobal.dataLoader = new DirectoryDataLoader();
		RGlobal.mapper = new ObjectMapper();
		RGlobal.data = new Database();
		RGlobal.dataLoader.addToDatabase("res/data");
		RGlobal.assetManager = new AssetManager();
		RGlobal.screens = new ScreenStack();
		RGlobal.keymap = new DefaultKeymap();
		RGlobal.constants = new Constants();

		WindowDataMDO data = (WindowDataMDO) RGlobal.data.getEntryByKey("window_data");
		
		LwjglApplicationConfiguration cfg = new LwjglApplicationConfiguration();
		cfg.title = data.windowName;
		cfg.useGL20 = false;
		cfg.width = data.defaultWidth;
		cfg.height = data.defaultHeight;
		
		new LwjglApplication(new RainfallGame(new DesktopFocusReporter()), cfg);
	}
}