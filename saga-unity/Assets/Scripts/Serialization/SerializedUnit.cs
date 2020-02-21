using System.Collections.Generic;

[System.Serializable]
public class SerializedUnit {

    public string dataKey;
    public string name;
    public string statusKey;
    public StatSet baseStats;
    public List<SerializedCombatItem> equipment;

    public SerializedUnit() {
        // serialized
    }

    public SerializedUnit(Unit unit) {
        dataKey = unit.DataKey;
        name = unit.Name;
        statusKey = unit.Status?.Data.Key;
        baseStats = unit.BaseStats;

        equipment = new List<SerializedCombatItem>();
        foreach (var item in unit.Equipment) {
            if (item == null) {
                equipment.Add(null);
            } else {
                equipment.Add(new SerializedCombatItem(item));
            }
        }
    }
}
