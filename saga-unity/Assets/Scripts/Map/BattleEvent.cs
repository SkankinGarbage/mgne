using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Sprite representations of Units that exist on the field. This class should be as dumb as
 * possible and just respond to animation and movement requests.
 */
[RequireComponent(typeof(CharaEvent))]
[DisallowMultipleComponent]
public class BattleEvent : MonoBehaviour {

    [HideInInspector]
    public Unit unitData;
    public BattleUnit unit { get; private set; }
    // public BattleController controller { get; private set; }

    public Vector2Int position { get { return GetComponent<MapEvent>().position; } }

    //public void Setup(BattleController controller, BattleUnit unit) {
    //    this.unit = unit;
    //    this.controller = controller;
    //}

    public void PopulateWithUnitData(Unit unitData) {
        this.unitData = unitData;
        if (unitData != null) {
            GetComponent<CharaEvent>().spritesheet = unitData.appearance;
            gameObject.name = unitData.unitName;
        }
    }
}
