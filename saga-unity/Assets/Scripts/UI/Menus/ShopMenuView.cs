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
    [SerializeField] private ListSelector mainMenu = null;

    [SerializeField] private string buySfxKey = "get";

    private ShopData data;

    public static ShopMenuView ShowDefault(ShopData data) {
        var menu = Instantiate<ShopMenuView>("Prefabs/UI/Shop/ShopMenu");
        menu.data = data;
        menu.Populate();
        return menu;
    }

    // doesn't do header text
    public void Populate(bool sellActive = false) {
        gp.Populate();

        buyList.Populate(data.items);
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
        headerText.text = "Welcome to " + data.shopName + ".";
        await expander.ShowRoutine();
        bool canceled = false;
        while (!canceled) {
            Populate(false);
            headerText.text = "Welcome to " + data.shopName + ".";
            buyList.Selector.ClearSelection();
            sellList.Selector.ClearSelection();
            var command = await mainMenu.SelectCommandAsync();
            switch (command) {
                case "Buy":
                    await BuyAsync();
                    break;
                case "Sell":
                    await SellAsync();
                break;
                default:
                    canceled = true;
                    break;
            }
        }
        await CloseRoutine();
    }

    private async Task BuyAsync() {
        while (true) {
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
                continue;
            } else if (Global.Instance().Data.Inventory.IsFull()) {
                headerText.text = "You can't carry that.";
                continue;
            }

            Global.Instance().Data.DeductGP(item.cost);
            Global.Instance().Data.Inventory.Add(new CombatItem(item));
            Populate(false);
            headerText.text = "Sold!";
            Global.Instance().Audio.PlaySFX(buySfxKey);
            await Global.Instance().Input.AwaitConfirm();
        }
    }

    private async Task SellAsync() {
        while (true) {
            Populate(true);
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
            Global.Instance().Audio.PlaySFX(buySfxKey);

            Global.Instance().Data.AddGP(item.cost);
            Global.Instance().Data.Inventory.Drop(selection);
        }
    }
}
