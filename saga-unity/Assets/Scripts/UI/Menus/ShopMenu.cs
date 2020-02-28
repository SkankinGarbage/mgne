using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuView : FullScreenMenuView {

    [SerializeField] private Text headerText = null;
    [SerializeField] private GoldView gp = null;
    [SerializeField] private CombatItemList buyList = null;
    [SerializeField] private CombatItemList inventoryList = null;
    [SerializeField] private CombatItemList sellList = null;
    [SerializeField] private ExpanderComponent expander = null;
    [SerializeField] private ListSelector menu = null;

    [SerializeField] private string buySfxKey = "pluck";

    private ShopData data;

    public static ShopMenuView ShowDefault(ShopData data) {
        var menu = Instantiate<ShopMenuView>("Prefabs/UI/Inn/InnMenu");
        menu.data = data;
        menu.Populate();
        return menu;
    }

    // doesn't do header text
    public void Populate(bool sellActive = false) {
        gp.Populate();

        buyList.Populate((IEnumerable<CombatItem>)data.items.GetEnumerator());
        if (sellActive) {
            sellList.gameObject.SetActive(true);
            inventoryList.gameObject.SetActive(false);
            sellList.Populate(Global.Instance().Data.Inventory);
        } else {
            sellList.gameObject.SetActive(false);
            inventoryList.gameObject.SetActive(true);
            inventoryList.Populate(Global.Instance().Data.Inventory);
        }
    }

    public async Task DoMenuAsync() {
        await expander.ShowRoutine();
        while (true) {
            buyList.Selector.ClearSelection();
            sellList.Selector.ClearSelection();
            var command = await menu.SelectCommandAsync();
            switch (command) {
                case "Buy":
                    await BuyAsync();
                    break;
                case "Sell":
                    await SellAsync();
                break;
                default:
                    return;
            }
        }
    }

    private async Task BuyAsync() {
        Populate(false);
        var selection = await buyList.Selector.SelectItemAsync(select => {
            headerText.text = data.items[select].itemDescription;
        }, true);
        if (selection < 0) {
            return;
        }
        var item = data.items[selection];
        
        if (item.cost > Global.Instance().Data.GP) {
            headerText.text = "You can't afford that.";
            return;
        } else if (Global.Instance().Data.Inventory.IsFull()) {
            headerText.text = "You can't carry that.";
            return;
        }
        headerText.text = "Sold!";

        Global.Instance().Data.DeductGP(item.cost);
        Global.Instance().Data.Inventory.Add(new CombatItem(item));
    }

    private async Task SellAsync() {
        Populate(true);
        while (true) {
            var selection = await sellList.Selector.SelectItemAsync(select => {
                headerText.text = data.items[select].itemDescription;
            }, true);
            if (selection < 0) {
                return;
            }
            var item = data.items[selection];

            if (item.cost <= 0) {
                headerText.text = "Can't sell that.";
                continue;
            }

            headerText.text = "Thanks.";

            Global.Instance().Data.AddGP(item.cost);
            Global.Instance().Data.Inventory.Drop(selection);
        }
    }
}
