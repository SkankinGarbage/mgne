/**
 *  SchemaModel.java
 *  Created on Dec 30, 2019 9:52:52 PM for project mgne-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mgnschema.wizards.export;

/**
 * The POBO that gets written out as JSON.
 */
public class SchemaClass {
	
	public String $type = "SchemaClass, Assembly-CSharp-Editor";
	public String name;
	public boolean excludeFromTree = false;
	public String path = "";
	
	public SchemaField[] fields;

	/** @see java.lang.Object#toString() */
	@Override public String toString() {
		return name;
	}
}
