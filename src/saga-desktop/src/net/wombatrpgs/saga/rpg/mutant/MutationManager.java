/**
 *  MutantLevelManager.java
 *  Created on May 24, 2014 8:31:41 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.mutant;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.battle.Battle;
import net.wombatrpgs.saga.rpg.chara.Chara;
import net.wombatrpgs.sagaschema.rpg.stats.Stat;

/**
 * Object that a mutant gets to manage what stats or levels it gets as a result
 * of battle by recording what happens to the mutant.
 */
public class MutationManager {
	
	protected static final int MHP_GAIN_MIN = 5;
	protected static final int MHP_GAIN_MAX = 19;
	
	protected Chara chara;
	protected Battle battle;
	protected int level;
	
	protected Map<MutantEvent, Integer> hits;
	
	/**
	 * Creates a new manager for a character. Note that the character should be
	 * a mutant and that the different races should probably be subclasses
	 * rather than an enum, oh well. Only specific to a single battle.
	 * @param	battle			The battle this manager records for
	 * @param	chara			The character to create for
	 */
	public MutationManager(Chara chara, Battle battle) {
		this.chara = chara;
		this.battle = battle;
		hits = new HashMap<MutantEvent, Integer>();
		level = battle.getLevel();
	}
	
	/**
	 * Records an event as having occurred.
	 * @param	event			The event that occurred
	 */
	public void recordEvent(MutantEvent event) {
		Integer existing = hits.get(event);
		if (existing == null) existing = 0;
		hits.put(event, existing + 1);
	}
	
	/**
	 * Finalize the event list and produce a list of available level up options.
	 * This should always be length 2 unless something changes? Should only be
	 * called once per instance.
	 * @return					A list of possible leveling options
	 */
	public List<Mutation> produceOptions() {
		List<MutantEvent> occurred = new ArrayList<MutantEvent>();
		for (MutantEvent event : hits.keySet()) {
			if (hits.get(event) > 0) occurred.add(event);
		}
		Mutation fixed = null;
		if (occurred.size() > 0) {
			switch (occurred.get(MGlobal.rand.nextInt(occurred.size()))) {
			case DAMAGED:
				fixed = SGlobal.settings.getMutations().genHealthOption(chara);
				break;
			case DAMAGED_PHYSICALLY:
				fixed = new MutationStat(chara, Stat.DEF);
				break;
			case USED_ABILITY:
				fixed = new MutationAbil(chara, level);
				break;
			case USED_AGI:
				fixed = new MutationStat(chara, Stat.AGI);
				break;
			case USED_MANA:
				fixed = new MutationStat(chara, Stat.MANA);
				break;
			case USED_STR:
				fixed = new MutationStat(chara, Stat.STR);
				break;
			}
		} else {
			fixed = SGlobal.settings.getMutations().randomOption(chara, level);
		}
		
		List<Mutation> results = new ArrayList<Mutation>();
		while (isMaxed(fixed)) {
			fixed = SGlobal.settings.getMutations().randomOption(chara, level);
		}
		results.add(fixed);
		
		for (int tries = 0; tries < 100; tries += 1) {
			Mutation random = SGlobal.settings.getMutations().randomOption(chara, level);
			if (fixed.getStat() != random.getStat() && !isMaxed(random)) {
				results.add(random);
				break;
			}
		}
		if (results.size() < 2) {
			results.add(new MutationAbil(chara, level));
		}
		
		return results;
	}
	
	/**
	 * Checks if a mutation has reached max stat.
	 * @param	mutation		The mutation to check
	 * @return					True if it is
	 */
	protected boolean isMaxed(Mutation mutation) {
		Stat stat = mutation.getStat();
		if (stat != null) {
			if (stat == Stat.MHP) {
				return chara.get(stat) >= 999;
			} else {
				return chara.get(stat) >= 99;
			}
		} else {
			return false;
		}
	}

}
