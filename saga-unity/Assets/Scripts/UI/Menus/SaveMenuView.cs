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

    /// <returns>The slot saved to or to load from, or -1 if none</returns>
    public async Task<int> DoMenuAsync(Mode mode) {
        int result = -1;
        while (true) {
            Populate(mode);
            var slot = await CellSelector.SelectItemAsync(null, true);
            if (slot < 0) {
                break;
            }
            
            if (mode == Mode.Load) {
                return slot;
            } else if (mode == Mode.Save) {
                Global.Instance().Serialization.SaveToSlot(slot);
                result = slot;
            }
        }

        await CloseRoutine();
        return result;
    }
}
