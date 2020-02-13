
using UnityEngine;

public class AbilEffectFactory {
    
    public static AbilEffect CreateEffect(AbilEffectData data, CombatItem item) {
        var dataType = data.GetType();
        if (typeof(EffectAttackData).IsAssignableFrom(dataType)) {
			return new EffectAttack((EffectAttackData) data, item);
        } else if (typeof(EffectNothingData).IsAssignableFrom(dataType)) {
			return new EffectNothing((EffectNothingData) data, item);
		} else if (typeof(EffectPassiveData).IsAssignableFrom(dataType)) {
			return new EffectPassive((EffectPassiveData) data, item);
		} else if (typeof(EffectFixedData).IsAssignableFrom(dataType)) {
			return new EffectFixed((EffectFixedData) data, item);
		} else if (typeof(EffectBoostData).IsAssignableFrom(dataType)) {
			return new EffectBoost((EffectBoostData) data, item);
		} else if (typeof(EffectDefendData).IsAssignableFrom(dataType)) {
			return new EffectDefend((EffectDefendData) data, item);
		} else if (typeof(EffectHealData).IsAssignableFrom(dataType)) {
			return new EffectHeal((EffectHealData) data, item);
		} else if (typeof(EffectMultihitData).IsAssignableFrom(dataType)) {
			return new EffectMultihit((EffectMultihitData) data, item);
		} else if (typeof(EffectStatusData).IsAssignableFrom(dataType)) {
			return new EffectStatus((EffectStatusData) data, item);
		} else if (typeof(EffectStatCandyData).IsAssignableFrom(dataType)) {
			return new EffectStatCandy((EffectStatCandyData) data, item);
		} else if (typeof(EffectDebuffData).IsAssignableFrom(dataType)) {
			return new EffectDebuff((EffectDebuffData) data, item);
		} else if (typeof(EffectReviveData).IsAssignableFrom(dataType)) {
			return new EffectRevive((EffectReviveData) data, item);
		} else if (typeof(EffectTeleportData).IsAssignableFrom(dataType)) {
			return new EffectTeleport((EffectTeleportData) data, item);
		} else if (typeof(EffectTransformData).IsAssignableFrom(dataType)) {
			return new EffectTransform((EffectTransformData) data, item);
		} else if (typeof(EffectUseRestoreData).IsAssignableFrom(dataType)) {
			return new EffectUseRestore((EffectUseRestoreData) data, item);
		}

        Debug.LogError("Unknown effect type" + dataType);
        return null;
    }
}