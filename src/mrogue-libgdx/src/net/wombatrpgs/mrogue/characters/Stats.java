/**
 *  Stats.java
 *  Created on Aug 19, 2013 5:15:10 PM for project rainfall-libgdx
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.mrogue.characters;

import net.wombatrpgs.mrogueschema.characters.data.StatsMDO;

/**
 * A collection of RPG stats associated with a character, item, etc. The idea is
 * that it can be added to another stats object in case of a boost, maintained
 * by a character to keep track of current stats, etc. Etc.
 */
public class Stats {
	
	private StatsMDO mdo;
	private int hp;
	
	/**
	 * Creates a new stats object from data.
	 * @param	stats			The stats data to create from
	 */
	public Stats(StatsMDO stats) {
		this.mdo = stats;
		hp = stats.mhp;
	}
	
	/** @return Max health points */
	public int getMHP() { return mdo.mhp; }
	
	/** @return Current health points */
	public int getHP() { return hp; }
	
	/** @return The speed modifier, as a float% */
	public float getSpeed() { return ((float) mdo.speed) / 100.f; }
	
	public int getVision() { return mdo.vision; }
	
	/**
	 * Inflicts a set amount of HP damage. Just subtraction here.
	 * @param	damage			The amount of damage inflicted
	 */
	public void takeDamage(int damage) {
		hp -= damage;
		if (hp < 0) hp = 0;
	}

}