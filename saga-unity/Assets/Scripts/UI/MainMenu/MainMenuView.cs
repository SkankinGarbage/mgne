using UnityEngine;
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

    private FadeData Fade => IndexDatabase.Instance().Fades.GetData("show_menu");

    public void OnEnable() {
        expander.Hide();
        Populate();
        MenuAync();
    }

    public void OnDisable() {
        
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

    private static MainMenuView defaultMenu;
    public static void ShowDefault() {
        if (defaultMenu == null) {
            defaultMenu = Instantiate(Resources.Load<GameObject>(PrefabPath)).GetComponent<MainMenuView>();
            defaultMenu.transform.SetParent(Global.Instance().UI.transform, worldPositionStays:false);
            //var rect = defaultMenu.GetComponent<RectTransform>();
            //rect.anchoredPosition = new Vector2(0, 0);
            //rect.offsetMax = new Vector2(0, 0);
            //rect.offsetMin = new Vector2(0, 0);

        }
        Global.Instance().Maps.avatar.PauseInput();
        defaultMenu.gameObject.SetActive(true);
    }
}
