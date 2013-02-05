/**
 *  Graphic.java
 *  Created on Feb 2, 2013 4:06:15 AM for project rainfall-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.rainfall.graphics;

import com.badlogic.gdx.assets.AssetManager;
import com.badlogic.gdx.graphics.Texture;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.graphics.g2d.TextureRegion;

import net.wombatrpgs.rainfall.core.Constants;
import net.wombatrpgs.rainfallschema.graphics.GraphicMDO;

/**
 * Single-frame graphics instance. Not meant to appear on its own, really.
 * Animations have their own version of this. This class is dumb.
 */
public class Graphic implements Queueable {
	
	protected GraphicMDO mdo;
	protected TextureRegion appearance;
	protected String filename;
	protected int width, height;
	
	/**
	 * Creates a new graphic from mdo data.
	 * @param 	mdo				The MDO with file data.
	 */
	public Graphic(GraphicMDO mdo) {
		this.mdo = mdo;
		this.filename = Constants.UI_DIR + mdo.file;
		this.width = mdo.width;
		this.height = mdo.height;
	}
	
	/**
	 * Creates a new graphic from a file path and width/height data.
	 * @param 	filename		The fully qualified filename to the .png
	 */
	public Graphic(String filename) {
		this.filename = filename;
		this.width = -1;
		this.height = -1;
	}

	/**
	 * @see net.wombatrpgs.rainfall.graphics.Queueable#queueRequiredAssets
	 * (com.badlogic.gdx.assets.AssetManager)
	 */
	@Override
	public void queueRequiredAssets(AssetManager manager) {
		manager.load(filename, Texture.class);
	}

	/**
	 * @see net.wombatrpgs.rainfall.graphics.Queueable#postProcessing
	 * (com.badlogic.gdx.assets.AssetManager, int)
	 */
	@Override
	public void postProcessing(AssetManager manager, int pass) {
		Texture tex = manager.get(filename, Texture.class);
		if (width == -1) width = tex.getWidth();
		if (height == -1) height = tex.getHeight();
		appearance = new TextureRegion(tex, 0, 0, width, height);
	}
	
	/** @return The basic width of this graphic (in px) */
	public int getWidth() { return this.width; }
	
	/** @return The basic height of this graphic (in px) */
	public int getHeight() { return this.height; }
	
	/**
	 * Returns the renderable portion of this image.
	 * @return					The preloaded texture region appearance
	 */
	public TextureRegion getGraphic() {
		return appearance;
	}
	
	/**
	 * Renders itself at a specific location.
	 * @param 	batch			The batch to render the graphic as part of
	 * @param 	x				The x-coord to render at (in px)
	 * @param 	y				The y-coord to render at (in px)
	 */
	public void renderAt(SpriteBatch batch, int x, int y) {
		batch.draw(appearance, x, y);
	}

}