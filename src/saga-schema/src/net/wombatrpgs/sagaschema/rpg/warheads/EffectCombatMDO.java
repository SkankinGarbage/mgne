/**
 *  EffectCombatMDO.java
 *  Created on Apr 25, 2014 11:34:24 AM for project saga-schema
 *  Author: psy_wombats
 *  Contact: psy_wombats@wombatrpgs.net
 */
package net.wombatrpgs.sagaschema.rpg.warheads;

import net.wombatrpgs.mgns.core.Annotations.DefaultValue;
import net.wombatrpgs.mgns.core.Annotations.Desc;
import net.wombatrpgs.mgns.core.Annotations.SchemaLink;
import net.wombatrpgs.sagaschema.rpg.abil.data.DamageType;
import net.wombatrpgs.sagaschema.rpg.abil.data.OffenseFlag;
import net.wombatrpgs.sagaschema.rpg.chara.MonsterFamilyMDO;

/**
 * Superclass for some common combat data.
 */
public abstract class EffectCombatMDO extends EffectEnemyTargetMDO {
	
	@Desc("Damage type")
	@DefaultValue("PHYSICAL")
	public DamageType damType;
	
	@Desc("Other flags")
	public OffenseFlag[] sideEffects;
	
	@Desc("Slayer families - monsters in these families will be weak")
	@SchemaLink(MonsterFamilyMDO.class)
	public String[] slayerFamiles;

}
