using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MainMenuView : MonoBehaviour {

    [SerializeField] private Text gpText = null;
    [SerializeField] private Text locationText = null;
    [SerializeField] private ListView partyCells = null;

    [SerializeField] private ListSelector mainMenu = null;
    [SerializeField] private ExpanderComponent expander = null;

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
            if (task.IsCanceled) {
                break;
            }

            switch (command) {
                // TODO:
                default:
                    break;
            }
        }
    }
}
