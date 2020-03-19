using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ItemMenuView : FullScreenMenuView, IItemUseableMenu {

    [SerializeField] private CombatItemList inventory = null;
    [SerializeField] private UnitList unitSelector = null;
    [SerializeField] private ListView unitCells = null;
    [SerializeField] private ListSelector useDropMenu = null;
    [SerializeField] private PointerLayer pointers = null;
    [SerializeField] private Text description = null;
    [SerializeField] private ListView collectablesList = null;

    public static ItemMenuView ShowDefault() {
        return Instantiate<ItemMenuView>("Prefabs/UI/Item/ItemMenu");
    }

    public void Populate() {
        inventory.Populate(Global.Instance().Data.Inventory);
        unitCells.Populate(Global.Instance().Party, (obj, unit) => {
            var cell = obj.GetComponent<UnitCellView>();
            cell.Populate(unit, pointers);
        });

        var displayCounts = new List<KeyValuePair<CollectableData, int>>(Global.Instance().Data.Collectables.DisplayCounts);
        if (displayCounts.Count > 0) {
            collectablesList.gameObject.SetActive(true);
            collectablesList.Populate(displayCounts, (obj, pair) => {
                obj.GetComponent<CollectableView>().Populate(pair.Key, pair.Value);
            });
        } else {
            collectablesList.gameObject.SetActive(false);
        }
    }

    public async Task DoMenuAsync() {
        var items = Global.Instance().Data.Inventory;
        while (true) {
            Populate();
            int slot = await inventory.Selector.SelectItemAsync(x => description.text = items[x]?.Data?.itemDescription, true);
            if (slot < 0) {
                break;
            }
            CombatItem item = items[slot];
            if (item == null) {
                continue;
            }
            useDropMenu.gameObject.SetActive(true);
            Vector3 offset = default;
            useDropMenu.transform.position = inventory.transform.GetChild(slot).transform.position + offset;
            string command = await useDropMenu.SelectCommandAsync();
            useDropMenu.gameObject.SetActive(false);
            switch (command) {
                case "Use":
                    if (item != null) {
                        if (item.IsMapUseable) {
                            await item.UseOnMapAsync(this, null);
                        } else {
                            AudioManager.PlayFail();
                            await Task.Delay(100);
                        }
                    }
                    break;
                case "Drop":
                    items.Drop(item);
                    break;
                default:
                    break;
            }
        }

        await CloseRoutine();
    }

    public Task<Unit> SelectUnitTargetAsync() {
        return unitSelector.SelectUnitTargetAsync();
    }

    public Task<bool> ConfirmSelectionAsync() {
        return unitSelector.selector.ConfirmSelectionAsync();
    }

    public void SelectAll() {
        unitSelector.selector.SelectAll();
    }

    public bool IsActive() {
        return unitSelector.gameObject.activeSelf;
    }

    public void SetActive(bool active) {
        unitSelector.gameObject.SetActive(active);
    }

    public void Repopulate() {
        Populate();
    }
}
