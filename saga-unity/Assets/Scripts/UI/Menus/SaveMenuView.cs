using UnityEngine;
using System.Threading.Tasks;

public class SaveMenuView : FullScreenMenuView {

    public enum Mode { Save, Load };

    [SerializeField] private ListView Cells = null;
    [SerializeField] private ListSelector CellSelector = null;

    public static SaveMenuView ShowDefault() {
        return Instantiate<SaveMenuView>("Prefabs/UI/SaveLoad/SaveLoadMenu");
    }

    public void Populate(Mode mode) {
        int slot = 1;
        Cells.Populate(Global.Instance().SystemData.SaveInfo, (obj, info) => {
            obj.GetComponent<SaveInfoView>().Populate(info, mode, slot);
            slot += 1;
        });
    }

    /// <returns>True if any saving/loading occurred</returns>
    public async Task<bool> DoMenuAsync(Mode mode) {
        bool? result = null;
        while (true) {
            Populate(mode);
            var slot = await CellSelector.SelectItemAsync(null, true);
            if (slot == -1) {
                break;
            }

            result = true; 
            if (mode == Mode.Load) {
                Global.Instance().Serialization.LoadGameDataForSlot(slot);
                break;
            } else if (mode == Mode.Save) {
                Global.Instance().Serialization.SaveToSlot(slot);
            }
        }

        await CloseRoutine();
        return result.Value;
    }
}
