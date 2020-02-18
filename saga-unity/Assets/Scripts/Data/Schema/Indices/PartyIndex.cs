using UnityEngine;
using System.Collections;

[UnityEngine.CreateAssetMenu(fileName = "PartyIndex", menuName = "Data/Index/Rpg/Party")]
public class PartyIndex : ScriptableObjectIndex<PartyData> {

    public PartyData defaultParty;
}
