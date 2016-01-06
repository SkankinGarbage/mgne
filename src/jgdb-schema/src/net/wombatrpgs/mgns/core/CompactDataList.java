/**
 *  CompactData.java
 *  Created on Jan 5, 2016 10:13:04 PM for project mgne-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgns.core;


/**
 * Compact schema + data storage format for the final project data blob. Just a
 * big ol list of every schema in the project. Not scaleable past your RAM.
 */
public class CompactDataList extends Schema {
	
	public CompactData[] data;

}
