using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;

public class AbilMenuView : FullScreenMenuView {

    [SerializeField] public UnitList miniSelect = null;
    [SerializeField] private UnitCellView unitCell = null;
    [SerializeField] private List<StatLabelView> statLabels = null;
    [SerializeField] private CombatItemList abilList = null;
    [SerializeField] private Text descriptionLabel = null;
    
    public static AbilMenuView ShowDefault() {
        return Instantiate<AbilMenuView>("Prefabs/UI/Abil/AbilMenu");
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
                    await item.UseOnMapAsync(this, unit);
                } else {
                    AudioManager.PlayFail();
                    await Task.Delay(100);
                }
            }
        }
    }
}
