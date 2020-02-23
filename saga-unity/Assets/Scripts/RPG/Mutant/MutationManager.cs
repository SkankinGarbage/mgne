using UnityEngine;
using System.Collections.Generic;

public class MutationManager : MonoBehaviour {

    protected const int MhpGainMin = 5;
    protected const int MhpGainMax = 19;

    protected Unit unit;
    protected Battle battle;
    protected int level;

    protected Dictionary<MutantEvent, int> hits;

    private const string MutationTablePath = "Database/MutationSettings/mutations_default";
    private static MutationSettingsData mutationsTable;
    public static MutationSettingsData MutationsTable {
        get {
            if (mutationsTable == null) {
                mutationsTable = Resources.Load<MutationSettingsData>(MutationTablePath);
            }
            return mutationsTable;
        }
    }

    public MutationManager(Unit unit, Battle battle) {
        this.unit = unit;
        this.battle = battle;
        hits = new Dictionary<MutantEvent, int>();
        level = battle.Enemy.HighestMeatLevel;
    }

    public void RecordEvent(MutantEvent @event) {
        var existing = 0;
        hits.TryGetValue(@event, out existing);
        hits[@event] = existing + 1;
    }

    public List<Mutation> GenerateOptions() {
        var occurred = new List<MutantEvent>();
        foreach (var @event in hits) {
			if (@event.Value > 0) {
                occurred.Add(@event.Key);
            }
        }

        Mutation @fixed = null;
		if (occurred.Count > 0) {
            var randomOccurred = occurred[Random.Range(0, occurred.Count)];
			switch (randomOccurred) {
			    case MutantEvent.DAMAGED:
				    @fixed = GenerateHealthOption();
				    break;
			    case MutantEvent.DAMAGED_PHYSICALLY:
				    @fixed = new StatMutation(unit, StatTag.DEF);
				    break;
			    case MutantEvent.USED_ABILITY:
				    @fixed = new AbilMutation(unit, level);
				    break;
			    case MutantEvent.USED_AGI:
				    @fixed = new StatMutation(unit, StatTag.AGI);
				    break;
			    case MutantEvent.USED_MANA:
				    @fixed = new StatMutation(unit, StatTag.MANA);
				    break;
			    case MutantEvent.USED_STR:
				    @fixed = new StatMutation(unit, StatTag.STR);
				    break;
			}
		} else {
			@fixed = GenerateRandomOption();
		}
		
		var results = new List<Mutation>();
		while (IsMaxed(@fixed)) {
			@fixed = GenerateRandomOption();
		}
		results.Add(@fixed);
		
		for (var tries = 0; tries < 100; tries += 1) {
            var random = GenerateRandomOption();
			if (@fixed.Stat != random.Stat && !IsMaxed(random)) {
				results.Add(random);
				break;
			}
		}
		if (results.Count < 2) {
			results.Add(new AbilMutation(unit, level));
		}
		
		return results;
	}
	
	protected bool IsMaxed(Mutation mutation) {
        var stat = mutation.Stat?.Tag;
        if (stat.HasValue) {
            if (stat == StatTag.MHP) {
                return unit.BaseStats[stat.Value] >= 999;
            } else {
                return unit.BaseStats[stat.Value] >= 99;
            }
        } else {
            return false;
        }
    }

    private Mutation GenerateHealthOption() {
        var gain = Random.Range(MhpGainMin, MhpGainMax);
        return new StatMutation(unit, StatTag.MHP, gain);
    }

    private Mutation GenerateRandomOption() {
        var spread = new List<MutantEvent>();
        for (var i = 0; i < MutationsTable.weightHp; i += 1) {
            spread.Add(MutantEvent.DAMAGED);
        }
        for (var i = 0; i < MutationsTable.weightDef; i += 1) {
            spread.Add(MutantEvent.DAMAGED_PHYSICALLY);
        }
        for (var i = 0; i < MutationsTable.weightAbil; i += 1) {
            spread.Add(MutantEvent.USED_ABILITY);
        }
        for (var i = 0; i < MutationsTable.weightAgi; i += 1) {
            spread.Add(MutantEvent.USED_AGI);
        }
        for (var i = 0; i < MutationsTable.weightStr; i += 1) {
            spread.Add(MutantEvent.USED_STR);
        }
        for (var i = 0; i < MutationsTable.weightMana; i += 1) {
            spread.Add(MutantEvent.USED_MANA);
        }
        switch (spread[Random.Range(0, spread.Count)]) {
            case MutantEvent.DAMAGED:               return GenerateHealthOption();
            case MutantEvent.DAMAGED_PHYSICALLY:    return new StatMutation(unit, StatTag.DEF);
            case MutantEvent.USED_ABILITY:          return new AbilMutation(unit, level);
            case MutantEvent.USED_AGI:              return new StatMutation(unit, StatTag.AGI);
            case MutantEvent.USED_MANA:             return new StatMutation(unit, StatTag.MANA);
            case MutantEvent.USED_STR:              return new StatMutation(unit, StatTag.STR);
            default: return null;
        }
    }
}
