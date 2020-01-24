using UnityEngine;
using System.Collections;

public class Unit {

    // careful, only guaranteed for NPCs, as heroes are dynamic
    protected CharaData data;

    public StatSet stats { get; private set; }

    /// <summary>
    /// Creates a new unit from serialized data. This should be the only point where units are
    /// created, as afterwise they'll be serialized and loaded up that way, or created on the fly
    /// for monsters
    /// </summary>
    public Unit(CharaData data) {
        this.data = data;
        stats = new StatSet(data.stats);
    }
}
