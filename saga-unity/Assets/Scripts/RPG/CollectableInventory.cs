using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

[JsonObject(MemberSerialization.OptIn)]
public class CollectableInventory {

    [JsonProperty] public Dictionary<string, int> CountsByKey { get; private set; }

    public IEnumerable<KeyValuePair<CollectableData, int>> DisplayCounts => CountsByKey.ToDictionary(
            pair => IndexDatabase.Instance().Collectables.GetData(pair.Key),
            pair => pair.Value).Where(pair => pair.Value > 0);

    public CollectableInventory() {
        CountsByKey = new Dictionary<string, int>();
    }

    public void AddItem(CollectableData collectable) {
        AddItem(collectable.Key);
    }
    public void AddItem(string collectableKey) {
        if (!CountsByKey.ContainsKey(collectableKey)) {
            CountsByKey[collectableKey] = 0;
        }
        CountsByKey[collectableKey] += 1;
    }

    public void RemoveItem(CollectableData collectable) {
        RemoveItem(collectable.Key);
    }
    public void RemoveItem(string collectableKey) {
        CountsByKey[collectableKey] -= 1;
    }
}
