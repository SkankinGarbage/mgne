﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MainMenuView : MonoBehaviour {

    private const string PrefabPath = "Prefabs/UI/MainMenu/MainMenu";

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
            defaultMenu = Instantiate(Resources.Load<GameObject>(PrefabPath)).GetComponent<MainMenuView>();
            defaultMenu.transform.SetParent(Global.Instance().UI.transform, worldPositionStays: false);
        }
        Global.Instance().Maps.avatar.PauseInput();
        defaultMenu.gameObject.SetActive(true);
    }

    public void Populate() {
        gpText.text = "GP: " + Global.Instance().Data.GP;
        locationText.text = "Floor: " + Global.Instance().Data.LocationName;

        partyCells.Populate(Global.Instance().Party, (obj, unit) => {
            var cell = obj.GetComponent<MainMenuCellView>();
            cell.Populate(unit);
        });
    }

    public async void MenuAync() {
        await expander.ShowRoutine();
        while (true) {
            var task = mainMenu.SelectCommandAsync();
            var command = await task;
            switch (command) {
                case "Abil":
                    await AbilSelect();
                    return;
                case null:
                    await expander.HideRoutine();
                    Close();
                    return;
            }
        }
    }

    public void Close() {
        gameObject.SetActive(false);
        Global.Instance().Maps.avatar.UnpauseInput();
    }

    private async Task AbilSelect() {
        mainMenu.ClearSelection();
        int slot = await abilMenu.SelectItemAsync();
        
        if (slot >= 0) {
            var unit = Global.Instance().Data.Party[slot];
            var abilMenu = AbilMenuView.ShowDefault();
            await abilMenu.DoMenuAsync(unit);
        }
    }
}