/**
 *  EffectMultihit.java
 *  Created on Apr 26, 2014 3:38:55 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.warheads;

import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.rpg.Battle;
import net.wombatrpgs.saga.rpg.Chara;
import net.wombatrpgs.saga.rpg.CombatItem;
import net.wombatrpgs.sagaschema.rpg.abil.data.warheads.EffectMultihitMDO;

/**
 * 3-heads, 8-legs, etc
 */
public class EffectMultihit extends EffectCombat {
	
	protected EffectMultihitMDO mdo;
	protected Battle lastUsedBattle;
	protected int sequence[];
	protected int seqIndex;

	/**
	 * Inherited constructor.
	 * @param	mdo				The data to create from
	 * @param	item			The item to create for
	 */
	public EffectMultihit(EffectMultihitMDO mdo, CombatItem item) {
		super(mdo, item);
		this.mdo = mdo;
		String hitsStrings[] = mdo.hits.split(",");
		sequence = new int[hitsStrings.length];
		for (int i = 0; i < hitsStrings.length; i += 1) {
			sequence[i] = Integer.valueOf(hitsStrings[i]);
		}
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.EffectCombat#calcPower
	 * (Battle, net.wombatrpgs.saga.rpg.Chara)
	 */
	@Override
	protected int calcPower(Battle battle, Chara user) {
		String tab = SConstants.TAB;
		if (lastUsedBattle == battle) {
			seqIndex += 1;
		} else {
			lastUsedBattle = battle;
			seqIndex = 0;
		}
		int hits = sequence[seqIndex];
		battle.println(tab + "Hit " + hits + " times.");
		return statDamage(user, mdo.attackStat, mdo.power) * hits;
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.EffectCombat#calcDamage
	 * (Battle, int, net.wombatrpgs.saga.rpg.Chara)
	 */
	@Override
	protected int calcDamage(Battle battle, int power, Chara target) {
		int hits = sequence[seqIndex];
		if (mdo.defendStat != null && !weak(target)) {
			power -= (target.get(mdo.defendStat) * hits);
		}
		return power;
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.EffectCombat#hits
	 * (net.wombatrpgs.saga.rpg.Battle, net.wombatrpgs.saga.rpg.Chara,
	 * net.wombatrpgs.saga.rpg.Chara, float)
	 */
	@Override
	protected boolean hits(Battle battle, Chara user, Chara target, float roll) {
		return true;
	}

}
