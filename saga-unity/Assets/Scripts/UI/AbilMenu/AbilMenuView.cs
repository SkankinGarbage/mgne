using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;

public class AbilMenuView : MonoBehaviour {

    private const string PrefabPath = "Prefabs/UI/Abil/AbilMenu";

    [SerializeField] private MainMenuCellView unitCell = null;
    [SerializeField] private List<StatLabelView> statLabels = null;
    [SerializeField] private CombatItemList abilList = null;
    [SerializeField] private Text descriptionLabel = null;
    [SerializeField] private MiniUnitSelectView miniSelect = null;
    
    public static AbilMenuView ShowDefault() {
        var menu = Instantiate(Resources.Load<GameObject>(PrefabPath)).GetComponent<AbilMenuView>();
        menu.transform.SetParent(Global.Instance().UI.transform, worldPositionStays: false);
        return menu;
    }

    public void Populate(Unit unit) {
        unitCell.Populate(unit);

        foreach (var label in statLabels) {
            label.Populate(unit.Stats);
        }
        abilList.Populate(unit.Equipment);
    }

    public async Task<int> SelectSlotAsync(Unit unit) {
        return await abilList.Selector.SelectItemAsync(slot => {
            var item = unit.Equipment[slot];
            descriptionLabel.text = item == null ? "" : item.Data.itemDescription;
        }, true);
    }

    public async Task DoMenuAsync(Unit unit) {
        while (true) {
            Populate(unit);
            var slot = await SelectSlotAsync(unit);
            if (slot == -1) {
                break;
            }
            var item = unit.Equipment[slot];
            if (item != null) {
                if (item.IsMapUseable) {
                    await item.OnMapUseDeferred(this);
                } else {
                    AudioManager.PlayFail();
                    await Task.Delay(100);
                }
            }
        }
    }

    public IEnumerator CloseRoutine() {
        Destroy(gameObject);
        yield break; ;
    }
}
