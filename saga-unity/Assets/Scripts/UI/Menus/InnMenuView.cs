using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;

public class InnMenuView : FullScreenMenuView {
    
    [SerializeField] private Text headerText = null;
    [SerializeField] private GoldView gp = null;
    [SerializeField] private ListView partyCells = null;
    [SerializeField] private ListSelector mainMenu = null;
    [SerializeField] private ExpanderComponent expander = null;

    [SerializeField] private string sfxKey = "inn";
    
    public static InnMenuView ShowDefault() {
        var menu = Instantiate<InnMenuView>("Prefabs/UI/Inn/InnMenu");
        menu.Populate();
        return menu;
    }

    public void Populate(bool healed = false) {
        var cost = CalculateCost();
        if (healed) {
            headerText.text = $"You're all set.";
        } else {
            headerText.text = $"It'll cost {cost} GP to spend the night.";
        }

        gp.Populate();

        partyCells.Populate(Global.Instance().Party, (obj, unit) => {
            var cell = obj.GetComponent<UnitCellView>();
            cell.Populate(unit);
        });
    }

    public async Task DoMenuAsync() {
        await expander.ShowRoutine();
        var command = await mainMenu.SelectCommandAsync();
        switch (command) {
            case "Pay":
                await TryHealAsync();
                break;
            default:
                break;
        }
        await CloseRoutine();
    }

    public override IEnumerator CloseRoutine() {
        yield return expander.HideRoutine();
        yield return base.CloseRoutine();
    }

    private async Task TryHealAsync() {
        if (Global.Instance().Data.GP >= CalculateCost()) {
            Global.Instance().Audio.PlaySFX(sfxKey);
            Global.Instance().Party.InnlikeHeal();
            Populate(healed: true);
            await Global.Instance().Input.AwaitConfirm();
        } else {
            headerText.text = $"You must may first.";
            await Global.Instance().Input.AwaitConfirm();
        }
    }

    private int CalculateCost() {
        return Global.Instance().Party.Aggregate(0, (runningCost, unit) => {
            return runningCost += unit[StatTag.MHP] - unit[StatTag.HP];
        });
    }
}
