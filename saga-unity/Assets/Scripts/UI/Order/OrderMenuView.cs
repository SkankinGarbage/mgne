using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;

public class OrderMenuView : MonoBehaviour {

    private const string PrefabPath = "Prefabs/UI/Order/OrderMenu";

    private const string ReorderCopy = "Reorder";
    private const string SwapCopy = "Swap with whom?";

    [SerializeField] private ListView unitList;
    [SerializeField] private DynamicListSelector unitSelector;
    [SerializeField] private DynamicListSelector destinationSelector;
    [SerializeField] private Text title;

    public static OrderMenuView ShowDefault() {
        var menu = Instantiate(Resources.Load<GameObject>(PrefabPath)).GetComponent<OrderMenuView>();
        menu.transform.SetParent(Global.Instance().UI.transform, worldPositionStays: false);
        return menu;
    }

    public void Populate() {
        unitList.Populate(Global.Instance().Party, (obj, unit) => {
            var cell = obj.GetComponent<ReorderCellView>();
            cell.Populate(unit, Global.Instance().Party.GetSlotForUnit(unit));
        });
        UpdateTitleCopy(false);
        destinationSelector.gameObject.SetActive(false);
    }

    public void UpdateTitleCopy(bool swapMode) {
        title.text = swapMode ? SwapCopy : ReorderCopy;
    }

    public async Task DoMenuAsync() {
        while (true) {
            Populate();
            UpdateTitleCopy(false);
            int slot1 = await unitSelector.SelectItemAsync(null, true);
            if (slot1 == -1) {
                break;
            }
            UpdateTitleCopy(true);

            int slot2 = await destinationSelector.SelectItemAsync();
            if (slot2 != -1 && slot1 != slot2) {
                Swap(slot1, slot2);
            }
        }
    }

    private void Swap(int slot1, int slot2) {
        Global.Instance().Party.SwapGroups(slot1, slot2);
    }
}
