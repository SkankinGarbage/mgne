using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;
using DG.Tweening;

public class OrderMenuView : FullScreenMenuView {

    private const string ReorderCopy = "Reorder";
    private const string SwapCopy = "Swap with whom?";

    [SerializeField] private ListView unitList = null;
    [SerializeField] private DynamicListSelector unitSelector = null;
    [SerializeField] private DynamicListSelector destinationSelector = null;
    [SerializeField] private Text title = null;

    public static OrderMenuView ShowDefault() {
        return Instantiate<OrderMenuView>("Prefabs/UI/Order/OrderMenu");
    }

    public void Populate() {
        unitList.Populate(Global.Instance().Party, (obj, unit) => {
            var cell = obj.GetComponent<ReorderCellView>();
            cell.Populate(unit, Global.Instance().Party.GroupIndexForUnit(unit));
        });
        UpdateTitleCopy(false);
    }

    public void UpdateTitleCopy(bool swapMode) {
        title.text = swapMode ? SwapCopy : ReorderCopy;
    }

    public async Task DoMenuAsync() {
        while (true) {
            Populate();
            UpdateTitleCopy(false);
            int slot1 = await unitSelector.SelectItemAsync(null, true);
            if (slot1 < 0) {
                break;
            }
            UpdateTitleCopy(true);

            destinationSelector.Selection = slot1;
            int slot2 = await destinationSelector.SelectItemAsync(null, true);
            if (slot2 != -1 && slot1 != slot2) {
                await SwapRoutine(slot1, slot2);
            }
            unitSelector.Selection = slot2;
            destinationSelector.ClearSelection();
        }
    }

    private IEnumerator SwapRoutine(int slot1, int slot2) {
        Global.Instance().Party.SwapGroups(slot1, slot2);
        if (slot1 > slot2) {
            var temp = slot1;
            slot1 = slot2;
            slot2 = temp;
        }
        var sprite1 = unitSelector.transform.GetChild(slot1).GetComponent<ReorderCellView>().Sprite;
        var sprite2 = unitSelector.transform.GetChild(slot2).GetComponent<ReorderCellView>().Sprite;
        var trans1 = sprite1.GetComponent<RectTransform>();
        var trans2 = sprite2.GetComponent<RectTransform>();
        sprite1.animates = true;

        int oldSel1 = unitSelector.Selection;
        int oldSel2 = destinationSelector.Selection;
        unitSelector.ClearSelection();
        destinationSelector.ClearSelection();

        float tilesPerSecond = 4.0f;
        yield return CoUtils.RunTween(trans1.DOMoveX(trans1.transform.position.x - 32, 1.0f / tilesPerSecond, true));
        sprite2.animates = true;
        float delta = Mathf.Abs(trans1.position.y - trans2.position.y);
        yield return CoUtils.RunParallel(new IEnumerator[] {
            CoUtils.RunTween(trans1.transform.DOMoveY(trans2.position.y, delta / tilesPerSecond / 32.0f, true)),
            CoUtils.RunTween(trans2.transform.DOMoveY(trans1.position.y, delta / tilesPerSecond / 32.0f, true)),
        }, this);
        sprite2.animates = false;
        yield return CoUtils.RunTween(trans1.transform.DOMoveX(trans1.transform.position.x + 32, 1.0f / tilesPerSecond, true));

        unitSelector.Selection = oldSel1;
        destinationSelector.Selection = oldSel2;
        sprite1.animates = false;
    }
}
