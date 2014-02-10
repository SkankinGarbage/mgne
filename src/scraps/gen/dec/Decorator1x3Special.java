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
import net.wombatrpgs.sagaschema.maps.decorators.Decorator1x3SpecialMDO;

/**
 * Generates a window thing.
 */
public class Decorator1x3Special extends DecoratorSingle {
	
	protected Decorator1x3SpecialMDO mdo;

	/**
	 * Generates a decorator from data.
	 * @param	mdo				The data to generate from
	 * @param	gen				The generator to generate for
	 */
	public Decorator1x3Special(Decorator1x3SpecialMDO mdo, MapGenerator gen) {
		super(mdo, gen);
		this.mdo = mdo;
	}

	/**
	 * @see net.wombatrpgs.saga.maps.gen.dec.Decorator#apply
	 * (net.wombatrpgs.saga.maps.Tile[][])
	 */
	@Override
	public void apply(Tile[][] tilesOld, Tile[][] tilesNew) {
		for (int x = 0; x < gen.getWidth(); x += 1) {
			for (int y = 0; y < gen.getHeight(); y += 1) {
				if (mdo.chance < gen.rand().nextFloat()) continue;
				if (!gen.isTile(tilesOld, mdo.bottomType, x, y-2)) continue;
				if (!gen.isTile(tilesOld, mdo.original, x, y-1)) continue;
				if (!gen.isTile(tilesOld, mdo.original, x, y)) continue;
				tilesNew[y][x] = SGlobal.tiles.getTile(mdo.t);
				tilesNew[y-1][x] = SGlobal.tiles.getTile(mdo.m);
				tilesNew[y-2][x] = SGlobal.tiles.getTile(mdo.b);
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
		SGlobal.tiles.requestTile(manager, mdo.t, mdo.original);
		SGlobal.tiles.requestTile(manager, mdo.m, mdo.original);
		SGlobal.tiles.requestTile(manager, mdo.b, mdo.bottomType);
	}

}