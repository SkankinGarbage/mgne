using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleController : MonoBehaviour {
    private const string ListenerId = "BattleControllerListenerId";

    // interally populated
    public Battle battle { get; private set; }
    public BattleUI ui { get; private set; }

    // internal state
    private Dictionary<BattleUnit, BattleEvent> dolls;

    // convenience getters
    public Map map { get { return GetComponent<Map>(); } }

    // === INITIALIZATION ==========================================================================

    public void Start() {
        // TODO: create this upon scene loading
        battle = new Battle();
        dolls = new Dictionary<BattleUnit, BattleEvent>();

        ui = FindObjectOfType<BattleUI>();
        if (ui == null) {
            ui = BattleUI.Spawn();
        }
    }

    // === GETTERS AND BOOKKEEPING =================================================================

    public BattleEvent GetDollForUnit(BattleUnit unit) {
        return dolls[unit];
    }
}
