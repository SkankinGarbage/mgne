using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks;

public class RecruitMenu : FullScreenMenuView {

    [SerializeField] private Text recruitSummary = null;
    [SerializeField] private ListView recruitCells = null;
    [SerializeField] private DynamicListSelector recruitSelector = null;
    [SerializeField] private Text infoText = null;

    private RecruitSelectionData data;

    public static AbilMenuView ShowDefault() {
        return Instantiate<AbilMenuView>("Prefabs/UI/Abil/RecruitMenu");
    }

    public void Populate(RecruitSelectionData data) {
        this.data = data;
        recruitSummary.text = data.title;
        var modifiedOptions = new List<CharaData>(data.options) {
            null // the "more info" cell hack
        };
        recruitCells.Populate(modifiedOptions, (obj, chara) => {
            obj.GetComponent<RecruitCellView>().Populate(chara);
        });
    }

    public async Task DoMenuAsync(RecruitSelectionData data) {
        Populate(data);
        while (true) {
            var slot = await recruitSelector.SelectItemAsync(null, true);
            if (slot >= data.options.Length) {
                await ShowInfo();
            } else {
                // todo
            }
        }
    }

    private async Task ShowInfo() {
        infoText.gameObject.SetActive(true);
        recruitCells.gameObject.SetActive(false);
        await Global.Instance().Input.AwaitConfirm();
        infoText.gameObject.SetActive(false);
        recruitCells.gameObject.SetActive(true);
    }
}
