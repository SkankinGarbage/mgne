using UnityEngine;
using UnityEngine.UI;

public class GoldView : MonoBehaviour {

    [SerializeField] private Text text = null;

    private string gpSuffix;

    public void Populate() {
        if (gpSuffix == null) gpSuffix = text.text;
        text.text = Global.Instance().Data.GP + " " + gpSuffix;
    }
}
