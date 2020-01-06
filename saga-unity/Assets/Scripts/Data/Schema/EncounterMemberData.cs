[System.Serializable]
public class EncounterMemberData {

    [UnityEngine.Tooltip("Enemy")]
    public CharaData enemy;

    [UnityEngine.Tooltip("Possible amount - max-min syntax, eg 0-2 or 1-4")]
    public string amount;
}
