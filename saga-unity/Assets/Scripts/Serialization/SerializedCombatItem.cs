[System.Serializable]
public class SerializedCombatItem {

    public string dataKey;
    public int usesRemaining;
    public int usesWhenAdded;

    public SerializedCombatItem() {
        // serialized
    }

    public SerializedCombatItem(CombatItem item) {
        dataKey = item.Data.key;
        usesRemaining = item.UsesRemaining;
        usesWhenAdded = item.UsesWhenAdded;
    }
}
