public class MonsterFamily {

    private MonsterFamilyData data;

    public string Key => data.key;

    public MonsterFamily(MonsterFamilyData data) {
        this.data = data;
    }

    public static void TransformEater(Unit eater, Unit dropper) {
        var result = GetTransformResult(eater, dropper);
        if (result != null) {
            eater.TransformInto(result);
        }
    }

    public static CharaData GetTransformResult(Unit eater, Unit dropper) {
        var otherFamily = dropper.MonsterFamily;
        if (otherFamily == null) {
            return null;
        }
        var meatGroup = otherFamily.FindMeatGroup();
        if (meatGroup == null) {
            return null;
        }
        

        // Find the outgoing link from our family with their meat group
        TransformationData link = null;
        if (eater.MonsterFamily == null) {
            return null;
        }
        foreach (var transform in eater.MonsterFamily.data.transformations) {
            if (meatGroup.key == transform.eat.key) {
                link = transform;
                break;
            }
        }

        // Nothing happens when we eat them
        if (link == null) {
            return null;
        }

        // Find the best power level fit for their meat power
        var power = dropper.MeatLevel;
        if (eater.MeatLevel > power) power = eater.MeatLevel;
        var resultFamily = link.result;
        var units = IndexDatabase.Instance().Units.GetAll();
		CharaData best = null;
        var bestPower = -1;
		foreach (var unitData in units) {
			if (resultFamily != unitData.family) continue;
			if (unitData.meatTargetLevel > power) continue;
			if (unitData.meatTargetLevel > bestPower) {
				best = unitData;
				bestPower = unitData.meatTargetLevel;
			}
		}
		
		// If no monster fits, take the lowest leveled in the family
		if (best == null) {
			foreach (var unitData in units) {
				if (resultFamily != unitData.family) continue;
				if (best == null || unitData.meatTargetLevel < bestPower) {
					best = unitData;
					bestPower = unitData.meatTargetLevel;
				}
			}
		}
		
		return best;
	}

    private MeatGroupData FindMeatGroup() {
        var groups = IndexDatabase.Instance().MeatGroups.GetAll();
        foreach (var group in groups) {
            foreach (var family in group.families) {
                if (family == data) {
                    return group;
                }
            }
        }
        return null;
    }
}
