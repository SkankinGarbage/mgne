using UnityEngine;
using System.Collections;

/// <summary>
/// Contains referenes to all the in-battle UI elements
/// </summary>
public class BattleUI : MonoBehaviour {

    private const string InstancePath = "Prefabs/UI/BattleUI";

    public static BattleUI Spawn() {
        GameObject obj = Instantiate(Resources.Load<GameObject>(InstancePath));
        return obj.GetComponent<BattleUI>();
    }
}
