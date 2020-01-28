using UnityEngine;
using System.Collections;

public class SelectableCell : MonoBehaviour {

    public GameObject selectedState;

    protected bool selectable = true;

    public virtual void SetSelected(bool selected) {
        selectedState.SetActive(selected);
    }

    public virtual void SetSelectable(bool selectable) {
        this.selectable = selectable;
    }

    public bool IsSelectable() {
        return selectable;
    }
}
