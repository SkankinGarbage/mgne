using UnityEngine;
using System.Collections;

[UnityEngine.CreateAssetMenu(fileName = "PartyIndex", menuName = "Data/Index/Rpg/PartyIndex")]
public class PartyIndex : ScriptableObjectIndex<PartyData> {

    public PartyData defaultParty;
}
