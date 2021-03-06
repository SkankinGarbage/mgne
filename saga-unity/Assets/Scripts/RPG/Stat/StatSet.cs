﻿using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * A stat set is a collection of stats of different types.
 * It can represent a modifier set (+3 str sword) or a base set (Alex, 10 str)
 * */
[System.Serializable]
public class StatSet : ISerializationCallbackReceiver {

    public StatDictionary serializedStats;
    private Dictionary<StatTag, float> stats;

    public StatSet() {
        InitNewSet();
    }

    public StatSet(StatSet other) : this() {
        foreach (StatTag tag in Enum.GetValues(typeof(StatTag))) {
            Stat stat = Stat.Get(tag);
            if (stat == null) {
                continue;
            }
            stats[tag] = other[tag];
        }
    }

    public StatSet(SerializedStatSet serialized) : this() {
        stats = new Dictionary<StatTag, float>();
        this[StatTag.STR] = serialized.str;
        this[StatTag.DEF] = serialized.def;
        this[StatTag.AGI] = serialized.agi;
        this[StatTag.MANA] = serialized.mana;
        this[StatTag.HP] = serialized.hp;
        this[StatTag.MHP] = serialized.mhp;
        if (serialized.flags != null) {
            foreach (StatTag flag in serialized.flags) {
                this[flag] = 1;
            }
            for (var i = 0; i < serialized.flagKeys?.Count; i += 1) {
                this[serialized.flagKeys[i]] = serialized.flagValues[i];
            }
        }
    }

    private StatSet(StatDictionary stats) : this() {
        Dictionary<string, float> statStrings = stats.ToDictionary();
        this.stats = new Dictionary<StatTag, float>();
        foreach (var stat in statStrings) {
            StatTag result;
            if (Enum.TryParse(stat.Key, true, out result)) {
                this.stats[result] = stat.Value;
            }
        }
    }

    private void InitNewSet() {
        stats = new Dictionary<StatTag, float>();
        foreach (StatTag tag in Enum.GetValues(typeof(StatTag))) {
            if (tag == StatTag.NONE) continue;
            Stat stat = Stat.Get(tag);
            if (stat == null) continue;
            stats[tag] = stat.Combinator.Identity();
        }
    }

    // === ACCESSORS ===

    public float Get(StatTag tag) {
        return stats[tag];
    }

    public bool Is(StatTag tag) {
        return stats[tag] > 0.0f;
    }

    public void Set(StatTag tag, float value) {
        stats[tag] = value;
    }

    public float this[StatTag tag] {
        get { return stats.ContainsKey(tag) ? stats[tag] : 0; }
        set { stats[tag] = value; }
    }

    // === OPERATIONS ===

    public void Add(StatTag tag, float value) {
        stats[tag] += value;
    }

    public void Sub(StatTag tag, float value) {
        Add(tag, -value);
    }

    public static StatSet operator +(StatSet a, StatSet b) => a.AddSet(b);
    public StatSet AddSet(StatSet other) {
        foreach (StatTag tag in Enum.GetValues(typeof(StatTag))) {
            if (tag == StatTag.NONE) continue;
            if (other.stats.ContainsKey(tag)) {
                float val1 = 0;
                float val2 = 0;
                stats.TryGetValue(tag, out val1);
                other.stats.TryGetValue(tag, out val2);
                stats[tag] = Stat.Get(tag).Combinator.Combine(val1, val2);
            }
        }
        return this;
    }

    public static StatSet operator -(StatSet a, StatSet b) => a.RemoveSet(b);
    public StatSet RemoveSet(StatSet other) {
        foreach (StatTag tag in Enum.GetValues(typeof(StatTag))) {
            if (tag == StatTag.NONE) continue;
            stats[tag] = Stat.Get(tag).Combinator.Decombine(stats[tag], other.stats[tag]);
        }
        return this;
    }

    // === SERIALIZATION ===

    public void OnBeforeSerialize() {
        Dictionary<string, float> statStrings = new Dictionary<string, float>();
        foreach (var stat in stats) {
            statStrings[stat.Key.ToString()] = stat.Value;
        }
        serializedStats = new StatDictionary(statStrings);
    }

    public void OnAfterDeserialize() {
        InitNewSet();
        if (serializedStats != null && !serializedStats.IsEmpty()) {
            AddSet(new StatSet(serializedStats));
        }
    }

    public static implicit operator StatSet(UnityEngine.Object v) {
        throw new NotImplementedException();
    }

    [Serializable]
    public class StatDictionary : SerialDictionary<string, float> {
        public StatDictionary(Dictionary<string, float> dictionary) : base(dictionary) {

        }
    }
}
