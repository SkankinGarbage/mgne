/**
 *  WarheadType.java
 *  Created on Feb 24, 2014 7:39:32 PM for project tactics-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.rpg.abil.data;

import java.util.EnumSet;

import net.wombatrpgs.sagaschema.rpg.chara.data.Resistable;
import net.wombatrpgs.sagaschema.rpg.stats.Flag;

/**
 * All damage falls into one of these categories.
 */
public enum DamageType implements Resistable {
	
	PHYSICAL		(EnumSet.noneOf(Flag.class),		EnumSet.of(Flag.RESIST_DAMAGE),							EnumSet.noneOf(Flag.class)),
	WEAPON			(EnumSet.of(Flag.IMMUNE_WEAPON),	EnumSet.of(Flag.RESIST_WEAPON, Flag.RESIST_DAMAGE),		EnumSet.noneOf(Flag.class)),
	
	FIRE			(EnumSet.of(Flag.RESIST_FIRE),		EnumSet.noneOf(Flag.class),								EnumSet.of(Flag.WEAK_FIRE)),
	ICE				(EnumSet.of(Flag.RESIST_ICE),		EnumSet.noneOf(Flag.class),								EnumSet.of(Flag.WEAK_ICE)),
	EARTH			(EnumSet.noneOf(Flag.class),		EnumSet.of(Flag.RESIST_EARTH),							EnumSet.of(Flag.WEAK_EARTH)),
	THUNDER			(EnumSet.noneOf(Flag.class),		EnumSet.of(Flag.RESIST_THUNDER),						EnumSet.of(Flag.WEAK_THUNDER)),
	
	NONELEMENTAL	(EnumSet.noneOf(Flag.class),		EnumSet.of(Flag.RESIST_TYPELESS),						EnumSet.noneOf(Flag.class));
	
	private EnumSet<Flag> immuneFlags;
	private EnumSet<Flag> resistFlags;
	private EnumSet<Flag> weakFlags;
	
	/**
	 * Internal enum constructor.
	 * @param	immuneFlags		All flags that grant full immunity
	 * @param	resistFlags		All flags that could be used to resist damage
	 * @param	weakFlags		All flags that indicate weakness
	 */
	DamageType(EnumSet<Flag> immuneFlags, EnumSet<Flag> resistFlags, EnumSet<Flag> weakFlags) {
		this.resistFlags = resistFlags;
		this.weakFlags = weakFlags;
		this.immuneFlags = immuneFlags;
	}

	/**
	 * @see net.wombatrpgs.sagaschema.rpg.chara.data.Resistable#getResistFlags()
	 */
	@Override
	public EnumSet<Flag> getResistFlags() {
		return resistFlags;
	}

	/**
	 * @see net.wombatrpgs.sagaschema.rpg.chara.data.Resistable#getWeakFlags()
	 */
	@Override
	public EnumSet<Flag> getWeakFlags() {
		return weakFlags;
	}

	/**
	 * @see net.wombatrpgs.sagaschema.rpg.chara.data.Resistable#getImmuneFlags()
	 */
	@Override
	public EnumSet<Flag> getImmuneFlags() {
		return immuneFlags;
	}
	
}
