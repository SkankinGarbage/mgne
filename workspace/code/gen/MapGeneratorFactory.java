/**
 *  MapGeneratorFactory.java
 *  Created on Oct 13, 2013 1:34:01 AM for project mrogue-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.maps.gen;

import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.maps.Level;
import net.wombatrpgs.sagaschema.maps.gen.MapGeneratorMDO;

/**
 * Maps makin' maps... I don't like it. But really this thing just creates the
 * creators based on an enum.
 */
public class MapGeneratorFactory {
	
	/**
	 * Creates and returns a map generator based on data. Specifically, checks
	 * the generation type of the incoming MDO.
	 * @param	mdo				The data to create generator from
	 * @param	parent			The map to generate into
	 */
	public static MapGenerator createGenerator(MapGeneratorMDO mdo, Level parent) {
		switch(mdo.generator) {
		case TEST:				return new GeneratorTest(mdo, parent);
		case CLASSIC_ROOMS:		return new GeneratorClassic(mdo, parent);
		case CELLULAR_INTERIOR:	return new GeneratorCellular(mdo, parent);
		}
		SGlobal.reporter.warn("No generator found for mdo type: " + mdo.generator);
		return new GeneratorTest(mdo, parent);
	}

}
