/**
 *  EffectRevive.java
 *  Created on May 31, 2015 9:08:04 PM for project saga-desktop
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
import net.wombatrpgs.saga.rpg.items.CombatItem;
import net.wombatrpgs.saga.screen.TargetSelectable;
import net.wombatrpgs.saga.ui.CharaSelector.SelectionListener;
import net.wombatrpgs.sagaschema.rpg.stats.Stat;
import net.wombatrpgs.sagaschema.rpg.warheads.EffectReviveMDO;

/**
 * Raises an ally (or allies?) from the dead.
 */
public class EffectRevive extends EffectAllyTarget {
	
	protected EffectReviveMDO mdo;
	
	/**
	 * Inherited constructor.
	 * @param	mdo				The data to create from
	 * @param 	item			The item invoking the effect
	 */
	public EffectRevive(EffectReviveMDO mdo, CombatItem item) {
		super(mdo, item);
		this.mdo = mdo;
	}

	/** @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#isMapUsable() */
	@Override public boolean isMapUsable() { return true; }

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
		
		boolean affected = false;
		for (Chara victim : targets) {
			if (victim.get(Stat.HP) <= 0) {
				String victimname = victim.getName();
				battle.println(tab + victimname + " is back to life.");
				victim.heal(calcPower(user));
				affected = true;
			}
		}
		if (!affected) {
			battle.println(tab + "Nothing happens.");
		}
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
	 * @see net.wombatrpgs.saga.rpg.warheads.AbilEffect#canTargetDead()
	 */
	@Override
	public boolean canTargetDead() {
		return true;
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
	 * Applies the resurrect effect like in battle, but on the world map. This
	 * performs the effect once the targets are finalized.
	 * @param	caller			The screen calling the effect
	 * @param	targets			The finalized list of targets
	 */
	protected void applyMapEffect(TargetSelectable caller, List<Chara> targets) {
		boolean affected = false;
		for (Chara victim : targets) {
			if (victim.get(Stat.HP) <= 0) {
				Chara user = caller.getUser() == null ? victim : caller.getUser();
				victim.heal(calcPower(user));
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
