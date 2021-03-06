package net.wombatrpgs.saga.core;

import net.wombatrpgs.mgne.core.Platform;
import net.wombatrpgs.mgne.core.SuperGame;
import net.wombatrpgs.mgne.core.interfaces.Reporter;
import net.wombatrpgs.mgne.core.reporters.PostReporter;

import com.badlogic.gdx.Files.FileType;
import com.badlogic.gdx.backends.lwjgl.LwjglApplication;
import com.badlogic.gdx.backends.lwjgl.LwjglApplicationConfiguration;

/**
 * Autogenerated by setup UI. This is the updated version for the new Saga
 * project.
 */
public class Main {
	
	public static final String WARMUP_NAME = "One moment please...";
	public static final int WARMUP_WIDTH = 320;
	public static final int WARMUP_HEIGHT = 240;
	
	/**
	 * Main launcher. Autogenerated at one point.
	 * @param 	args		Unusused
	 */
	public static void main(String[] args) {
		
		LwjglApplicationConfiguration cfg = new LwjglApplicationConfiguration();
		cfg.title = WARMUP_NAME;
		cfg.useGL30 = false;
		cfg.width = WARMUP_WIDTH;
		cfg.height = WARMUP_HEIGHT;
		cfg.stencil = 8;
		cfg.resizable = false;
		// dammit I want the old seticon back!!
		cfg.addIcon("res/ui/icon_128.png", FileType.Internal);
		cfg.addIcon("res/ui/icon_32.png", FileType.Internal);
		cfg.addIcon("res/ui/icon_16.png", FileType.Internal);
		
		new LwjglApplication(new SuperGame(
				new SagaGame(),
				new Platform() {
					@Override public Reporter getReporter() {
						return new PostReporter();
					}
		}, args), cfg);
	}
}
