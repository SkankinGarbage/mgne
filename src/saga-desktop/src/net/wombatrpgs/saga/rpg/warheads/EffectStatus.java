/**
 *  EffectStatus.java
 *  Created on Apr 28, 2014 7:09:32 PM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.warheads;

import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.rpg.battle.Battle;
import net.wombatrpgs.saga.rpg.battle.Intent;
import net.wombatrpgs.saga.rpg.chara.Chara;
import net.wombatrpgs.saga.rpg.chara.Status;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.sagaschema.rpg.warheads.EffectStatusMDO;

/**
 * Inflicts status conditions on enemies.
 */
public class EffectStatus extends EffectEnemyTarget {
	
	protected EffectStatusMDO mdo;
	protected Status status;

	/**
	 * Inherited constructor.
	 * @param	mdo				The data to construct from
	 * @param	item			The item to construct for
	 */
	public EffectStatus(EffectStatusMDO mdo, CombatItem item) {
		super(mdo, item);
		this.mdo = mdo;
		status = Status.get(mdo.status);
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.EffectEnemyTarget#onAffect
	 * (net.wombatrpgs.saga.rpg.battle.Battle, net.wombatrpgs.saga.rpg.battle.Intent,
	 * net.wombatrpgs.saga.rpg.chara.Chara, net.wombatrpgs.saga.rpg.chara.Chara, int)
	 */
	@Override
	protected void onAffect(Battle battle, Intent intent, Chara user, Chara victim, int power) {
		status.inflict(battle, victim);
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.EffectEnemyTarget#calcPower
	 * (net.wombatrpgs.saga.rpg.battle.Battle, net.wombatrpgs.saga.rpg.chara.Chara)
	 */
	@Override
	protected int calcPower(Battle battle, Chara user) {
		int power = mdo.hit;
		if (mdo.accStat != null) {
			power += user.get(mdo.accStat);
		}
		return power;
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.EffectEnemyTarget#hits
	 * (net.wombatrpgs.saga.rpg.battle.Battle, net.wombatrpgs.saga.rpg.chara.Chara,
	 * net.wombatrpgs.saga.rpg.chara.Chara, int, float)
	 */
	@Override
	protected boolean hits(Battle battle, Chara user, Chara target, int power, float roll) {
		String tab = SConstants.TAB;
		int temp = power;
		if (target.getStatus() != null) {
			if (target.getStatus().getPriority() > status.getPriority()) {
				temp -= 255;
			}
		}
		if (target.resists(status)) {
			temp -= 255;
		}
		if (mdo.evadeStat != null) {
			temp -= target.get(mdo.evadeStat);
		}
		float chance = (float) temp / 100f;
		boolean hit = roll < chance;
		if (hit) {
			return true;
		} else {
			battle.println(tab + "Nothing happens.");
			return false;
		}
	}

}
