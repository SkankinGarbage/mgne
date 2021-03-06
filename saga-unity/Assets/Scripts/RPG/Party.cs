﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class Party : IEnumerable<Unit> {

    [JsonProperty] public List<List<Unit>> Groups { get; private set; }

    public Unit this[int slot] { get => Groups[slot][0]; }
    
    public IEnumerable<Unit> Members => Groups.SelectMany(unit => unit);
    public Unit Leader => IsAnyAlive ? Members.First(unit => unit.IsAlive) : Members.FirstOrDefault();
    public int Size => Members.Count();
    public int HighestMeatLevel => Members.Max(unit => unit.MeatLevel);
    public bool HasFlag(StatTag flag) => AnyMembersMeetCritera(unit => unit[flag] > 0);
    public bool IsCarryingItemType(CombatItemData itemData) => AnyMembersMeetCritera(unit => unit.IsCarryingItemType(itemData));
    public bool IsAnyAlive => Members.Any(unit => unit.IsAlive);

    public IEnumerator<Unit> GetEnumerator() { return Members.GetEnumerator(); }
    IEnumerator IEnumerable.GetEnumerator() { return Members.GetEnumerator(); }

    public Party() {
        Groups = new List<List<Unit>>();
    }

    public Party(EncounterData encounter) : this() {
        foreach (var member in encounter.members) {
            var group = new List<Unit>();
            int amount;
            if (member.amount.Contains('-')) {
                var split = member.amount.Split('-');
                amount = Random.Range(int.Parse(split[0]), int.Parse(split[1]) + 1);
            } else {
                amount = int.Parse(member.amount);
            }
            if (amount > 0) {
                for (int i = 0; i < amount; i += 1) {
                    group.Add(new Unit(member.enemy));
                }
                Groups.Add(group);
            }
        }
    }

    public Party(PartyData data) : this() {
        foreach (var entry in data.members) {
            var group = new List<Unit>();
            for (var i = 0; i < entry.count; i += 1) {
                group.Add(new Unit(entry.monster));
            }
            Groups.Add(group);
        }
    }

    public Party Copy() {
        Party copy = new Party();
        foreach (var group in Groups) {
            var newGroup = new List<Unit>();
            foreach (var unit in group) {
                newGroup.Add(unit.Copy());
            }
            copy.Groups.Add(newGroup);
        }
        return copy;
    }

    public bool Contains(Unit toFind) {
        return IndexFor(toFind) >= 0;
    }

    public void AddMember(Unit member) {
        Groups.Add(new List<Unit>() { member });
    }

    public void RemoveMember(Unit member) {
        var index = GroupIndexForUnit(member);
        if (Groups[index].Count == 1) {
            RemoveGroup(index);
        } else {
            Groups[index].Remove(member);
        }
    }

    public void RemoveGroup(int groupIndex) {
        Groups.RemoveAt(groupIndex);
    }

    /// <returns>The index of the group containing the given unit, or -1 if not found</returns>
    public int IndexFor(Unit toFind) {
        for (var i = 0; i < Groups.Count; i += 1) {
            if (Groups[i].Contains(toFind)) {
                return i;
            }
        }
        return -1;
    }

    public void SwapGroups(int index1, int index2) {
        int firstIndex, secondIndex;
        if (index1 < index2) {
            firstIndex = index1;
            secondIndex = index2;
        } else {
            firstIndex = index2;
            secondIndex = index1;
        }
        List<Unit> group1 = Groups[firstIndex];
        List<Unit> group2 = Groups[secondIndex];
        Groups.RemoveAt(secondIndex);
        Groups.RemoveAt(firstIndex);
        Groups.Insert(firstIndex, group2);
        Groups.Insert(secondIndex, group1);
    }

    public void Swap(Unit unit1, Unit unit2) {
        SwapGroups(IndexFor(unit1), IndexFor(unit2));
    }

    /// <summary>
    /// Weighted RNG roll to target a group in this party. Favors groups in the front of the party.
    /// </summary>
    public List<Unit> SelectFrontfacingGroup() {
        List<List<Unit>> weightedGroups = new List<List<Unit>>();
        var added = 0;
        for (int i = 0; i < Groups.Count(); i += 1) {
            List<Unit> group = Groups[i];
            bool active = false;
            foreach (var member in group) {
                if (!member.IsDead) {
                    active = true;
                    break;
                }
            }
            if (!active) continue;
            int weight;
            switch (added) {
                case 0: weight = 3; break;
                case 1: weight = 2; break;
                default: weight = 1; break;
            }
            for (int j = 0; j < weight; j += 1) {
                weightedGroups.Add(group);
            }
            added += 1;
        }
        int index = Random.Range(0, weightedGroups.Count);
        return weightedGroups[index];
    }

    /// <summary>
    /// Restores the HP of all characters in the party, and puts any of their innate abilities up to max uses. This 
    /// isn't really a full heal because it doesn't touch status conditions.
    /// </summary>
    public void InnlikeHeal() {
        foreach (var member in Members) {
            member.RestoreHP();
            member.RestoreAbilityUses();
        }
    }

    /// <returns>The ordinal of the group the unit is in, or -1 for not found</returns>
    public int GroupIndexForUnit(Unit unit) {
        for (var i = 0; i < Groups.Count; i += 1) {
            if (Groups[i].Contains(unit)) {
                return i;
            }
        }
        return -1;
    }

    private bool AnyMembersMeetCritera(Func<Unit, bool> critera) {
        foreach (var unit in Members) {
            if (critera(unit)) {
                return true;
            }
        }
        return false;
    }
}
