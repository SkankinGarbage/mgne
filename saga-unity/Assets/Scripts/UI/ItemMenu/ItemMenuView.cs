using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ItemMenuView : MonoBehaviour {

    private const string PrefabPath = "Prefabs/UI/Item/ItemMenu";

    [SerializeField] private CombatItemList inventory = null;
    [SerializeField] private DynamicListSelector unitSelector = null;
    [SerializeField] private ListView unitCells = null;
    [SerializeField] private ListSelector useDropMenu = null;
    [SerializeField] private PointerLayer pointers = null;
    [SerializeField] private Text description = null;

    public static ItemMenuView ShowDefault() {
        var menu = Instantiate(Resources.Load<GameObject>(PrefabPath)).GetComponent<ItemMenuView>();
        menu.transform.SetParent(Global.Instance().UI.transform, worldPositionStays: false);
        return menu;
    }

    public void Populate() {
        inventory.Populate(Global.Instance().Data.Inventory);
        unitCells.Populate(Global.Instance().Party, (obj, unit) => {
            var cell = obj.GetComponent<MiniUnitCellView>();
            cell.Populate(unit, pointers);
        });
    }

    public async Task DoMenuAsync() {
        var items = Global.Instance().Data.Inventory;
        while (true) {
            Populate();
            int slot = await inventory.Selector.SelectItemAsync(x => description.text = items[x]?.Data?.itemDescription, true);
            if (slot == -1) {
                break;
            }
            CombatItem item = items[slot];
            if (item == null) {
                continue;
            }
            useDropMenu.gameObject.SetActive(true);
            Vector3 offset = default;
            useDropMenu.transform.position = inventory.transform.GetChild(slot).transform.position + offset;
            string command = await useDropMenu.SelectCommandAsync();
            switch (command) {
                case "Use":
                    // TODO
                    break;
                case "Drop":
                    items.Drop(item);
                    break;
                default:
                    break;
            }
            useDropMenu.gameObject.SetActive(false);
        }

        await CloseRoutine();
    }

    public IEnumerator CloseRoutine() {
        Destroy(gameObject);
        yield break; ;
    }
}
