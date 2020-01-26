using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour {

    [SerializeField] private Text gpText = null;
    [SerializeField] private Text locationText = null;
    [SerializeField] private ListView partyCells = null;

    public void OnEnable() {
        Populate();
    }

    public void Populate() {
        gpText.text = "GP: " + Global.Instance().Data.GP;
        locationText.text = "Floor: " + Global.Instance().Data.LocationName;

        partyCells.Populate(Global.Instance().Party, (obj, unit) => {
            var cell = obj.GetComponent<MainMenuCellView>();
            cell.Populate(unit);
        });
    }
}
