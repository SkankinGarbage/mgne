using UnityEngine;

public class MiniCharaSelectView : MonoBehaviour {

    [SerializeField] private DynamicListSelector selector = null;
    [SerializeField] private ListView partyCells = null;

    public void Populate() {
        partyCells.Populate(Global.Instance().Party, (obj, unit) => {
            var cell = obj.GetComponent<MiniCharaCellView>();
            cell.Populate(unit);
        });
    }
}