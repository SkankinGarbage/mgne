/**
 *  EffectHeal.java
 *  Created on Apr 26, 2014 10:15:42 AM for project saga-desktop
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.saga.rpg.warheads;

import java.util.ArrayList;
import java.util.List;

import net.wombatrpgs.mgne.core.MGlobal;
import net.wombatrpgs.saga.core.SConstants;
import net.wombatrpgs.saga.core.SGlobal;
import net.wombatrpgs.saga.rpg.battle.Battle;
import net.wombatrpgs.saga.rpg.battle.Intent;
import net.wombatrpgs.saga.rpg.chara.Chara;
import net.wombatrpgs.saga.rpg.chara.Status;
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.saga.screen.TargetSelectable;
import net.wombatrpgs.saga.ui.CharaSelector.SelectionListener;
import net.wombatrpgs.sagaschema.rpg.abil.data.UseRestoreType;
import net.wombatrpgs.sagaschema.rpg.warheads.EffectHealMDO;

/**
 * This is for the Rod.
 */
public class EffectHeal extends EffectAllyTarget {
	
	protected EffectHealMDO mdo;

	public EffectHeal(EffectHealMDO mdo, CombatItem item) {
		super(mdo, item);
		this.mdo = mdo;
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.EffectAllyTarget#isBattleUsable()
	 */
	@Override
	public boolean isBattleUsable() {
		// kind of a hack w/e
		return mdo.useRestore != UseRestoreType.RESTORES_USES;
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#isMapUsable()
	 */
	@Override
	public boolean isMapUsable() {
		return true;
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#onMapUse
	 * (net.wombatrpgs.saga.screen.TargetSelectable)
	 */
	@Override
	public void onMapUse(final TargetSelectable caller) {
		final List<Chara> targets = new ArrayList<Chara>();
		switch (mdo.projector) {
		case ALLY_PARTY: case PLAYER_PARTY_ENEMY_GROUP:
			targets.addAll(SGlobal.heroes.getAll());
			applyMapEffect(caller, targets);
			break;
		case SINGLE_ALLY:
			caller.awaitSelection(new SelectionListener() {
				@Override public boolean onSelection(Chara selected) {
					if (selected != null) {
						targets.add(selected);
						applyMapEffect(caller, targets);
					}
					return true;
				}
			});
			break;
		case USER:
			if (caller.getUser() != null) {
				targets.add(caller.getUser());
				applyMapEffect(caller, targets);
			}
			break;
		}
	}

	/**
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#resolve
	 * (net.wombatrpgs.saga.rpg.battle.Intent)
	 */
	@Override
	public void resolve(Intent intent) {
		Battle battle = intent.getBattle();
		Chara user = intent.getActor();
		String username = user.getName();
		CombatItem item = intent.getItem();
		String itemname = item.getName();
		String tab = SConstants.TAB;
		List<Chara> targets = new ArrayList<Chara>();
		switch (mdo.projector) {
		case ALLY_PARTY: case PLAYER_PARTY_ENEMY_GROUP:
			targets.addAll(intent.getTargets());
			battle.println(username + " uses " + itemname + ".");
			break;
		case SINGLE_ALLY: case USER:
			Chara target = intent.getTargets().get(0);
			String targetname = target.getName();
			targets.add(target);
			if (target == user) {
				battle.println(username + " uses " + itemname + ".");
			} else {
				battle.println(username + " uses " + itemname + " on " + targetname + ".");
			}
			break;
		}
		
		for (Chara victim : targets) {
			String victimname = victim.getName();
			int heal = victim.heal(calcPower(user));
			if (heal > 0) {
				battle.println(tab + victimname + " recovered " + heal + " HP.");
			}
			Status status = victim.getStatus();
			if (status != null && status.isContainedIn(mdo.heals)) {
				status.heal(battle, victim);
			} else if (heal <= 0) {
				battle.println(tab + "Nothing happens.");
			}
		}
	}
	
	/**
	 * Calculates the power of a character's heal.
	 * @param	user			The character casting the heal
	 * @return					The value of that heal, in HP
	 */
	protected int calcPower(Chara user) {
		int heal = mdo.power;
		if (mdo.powerStat != null) {
			heal *= user.get(mdo.powerStat);
		}
		heal += mdo.base;
		return heal;
	}
	
	/**
	 * Applies the healing effect like in battle, but on the world map. This
	 * performs the effect once the targets are finalized.
	 * @param	caller			The screen calling the effect
	 * @param	targets			The finalized list of targets
	 */
	protected void applyMapEffect(TargetSelectable caller, List<Chara> targets) {
		boolean affected = false;
		for (Chara victim : targets) {
			Chara user = caller.getUser() == null ? victim : caller.getUser();
			if (victim.heal(calcPower(user)) > 0) {
				affected = true;
			}
			Status status = victim.getStatus();
			if (status != null && status.isContainedIn(mdo.heals)) {
				status.heal(null, victim);
				affected = true;
			}
			if (mdo.useRestore == UseRestoreType.RESTORES_USES) {
				victim.restoreAbilUses();
				affected = true;
			}
		}
		if (affected) {
			item.deductUse();
			MGlobal.audio.playSFX(SConstants.SFX_CURE);
			caller.refresh();
		} else {
			MGlobal.audio.playSFX(SConstants.SFX_FAIL);
		}
	}

}
