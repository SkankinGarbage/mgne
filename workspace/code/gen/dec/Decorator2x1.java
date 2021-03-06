/**
 *  Decorator3x3.java
 *  Created on Oct 13, 2013 6:28:32 AM for project mrogue-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.maps.gen.dec;

import com.badlogic.gdx.assets.AssetManager;

import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.maps.Tile;
import net.wombatrpgs.saga.maps.gen.MapGenerator;
import net.wombatrpgs.sagaschema.maps.decorators.Decorator2x1MDO;

/**
 * Generates a wall hanger thing.
 */
public class Decorator2x1 extends DecoratorSingle {
	
	protected Decorator2x1MDO mdo;

	/**
	 * Generates a decorator from data.
	 * @param	mdo				The data to generate from
	 * @param	gen				The generator to generate for
	 */
	public Decorator2x1(Decorator2x1MDO mdo, MapGenerator gen) {
		super(mdo, gen);
		this.mdo = mdo;
	}

	/**
	 * @see net.wombatrpgs.saga.maps.gen.dec.Decorator#apply
	 * (net.wombatrpgs.saga.maps.Tile[][])
	 */
	@Override
	public void apply(Tile[][] tilesOld, Tile[][] tilesNew) {
		this.tilesNew = tilesNew;
		for (int x = 0; x < gen.getWidth(); x += 1) {
			for (int y = 0; y < gen.getHeight(); y += 1) {
				if (mdo.chance < gen.rand().nextFloat()) continue;
				if (!legal(tilesOld, x, y)) continue;
				if (!legal(tilesOld, x+1, y)) continue;
				tilesNew[y][x] = SGlobal.tiles.getTile(mdo.l);
				tilesNew[y][x+1] = SGlobal.tiles.getTile(mdo.r);
			}
		}
	}
	
	/**
	 * @see net.wombatrpgs.saga.maps.gen.dec.Decorator#queueRequiredAssets
	 * (com.badlogic.gdx.assets.AssetManager)
	 */
	@Override
	public void queueRequiredAssets(AssetManager manager) {
		super.queueRequiredAssets(manager);
		SGlobal.tiles.requestTile(manager, mdo.l, replace);
		SGlobal.tiles.requestTile(manager, mdo.r, replace);
	}

}
