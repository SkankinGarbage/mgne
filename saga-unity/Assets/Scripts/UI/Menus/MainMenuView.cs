using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MainMenuView : FullScreenMenuView {

    [SerializeField] private Text gpText = null;
    [SerializeField] private Text locationText = null;
    [SerializeField] private ListView partyCells = null;

    [SerializeField] private ListSelector mainMenu = null;
    [SerializeField] private ExpanderComponent expander = null;
    [SerializeField] private GridSelector abilMenu = null;

    private FadeData Fade => IndexDatabase.Instance().Fades.GetData("show_menu");

    public void OnEnable() {
        expander.Hide();
        Populate();
        MenuAync();
    }

    private static MainMenuView defaultMenu;
    public static void ShowDefault() {
        if (defaultMenu == null) {
            defaultMenu = Instantiate<MainMenuView>("Prefabs/UI/MainMenu/MainMenu");
        }
        Global.Instance().Maps.Avatar.PauseInput();
        defaultMenu.gameObject.SetActive(true);
    }

    public void Populate() {
        gpText.text = "GP: " + Global.Instance().Data.GP;
        locationText.text = "Floor: " + Global.Instance().Data.LocationName;

        partyCells.Populate(Global.Instance().Party, (obj, unit) => {
            var cell = obj.GetComponent<UnitCellView>();
            cell.Populate(unit);
        });
    }

    public async void MenuAync() {
        await expander.ShowRoutine();
        while (true) {
            var command = await mainMenu.SelectCommandAsync();
            switch (command) {
                case "Abil":
                    await AbilSelect();
                    break;
                case "Equip":
                    await EquipSelect();
                    break;
                case "Items":
                    await ItemSelect();
                    break;
                case "Order":
                    await OrderSelect();
                    break;
                case "Save":
                    await SaveSelect();
                    break;
                case null:
                    await expander.HideRoutine();
                    Close();
                    return;
            }
        }
    }

    public void Close() {
        gameObject.SetActive(false);
        Global.Instance().Maps.Avatar.UnpauseInput();
    }

    private async Task AbilSelect() {
        mainMenu.ClearSelection();
        int slot = await abilMenu.SelectItemAsync();
        
        if (slot >= 0) {
            var unit = Global.Instance().Data.Party[slot];
            var abilMenu = AbilMenuView.ShowDefault();
            await abilMenu.DoMenuAsync(unit);
            await abilMenu.CloseRoutine();
        }
    }

    private async Task EquipSelect() {
        mainMenu.ClearSelection();
        int slot = await abilMenu.SelectItemAsync();

        if (slot >= 0) {
            var unit = Global.Instance().Data.Party[slot];
            var equipMenu = EquipMenuView.ShowDefault();
            await equipMenu.DoMenuAsync(unit);
            await equipMenu.CloseRoutine();
        }
    }

    private async Task ItemSelect() {
        var itemMenu = ItemMenuView.ShowDefault();
        itemMenu.Populate();
        await itemMenu.DoMenuAsync();
    }

    private async Task OrderSelect() {
        if (Global.Instance().Data.GetSwitch("disable_reorder")) {
            Global.Instance().Audio.PlaySFX("fail");
            return;
        }
        var orderMenu = OrderMenuView.ShowDefault();
        orderMenu.Populate();
        await orderMenu.DoMenuAsync();
    }

    private async Task SaveSelect() {
        var saveMenu = SaveMenuView.ShowDefault();
        await saveMenu.DoMenuAsync(SaveMenuView.Mode.Save);
    }
}
