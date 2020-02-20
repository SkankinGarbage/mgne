using UnityEngine;
using System.Collections;

public abstract class FullScreenMenuView : MonoBehaviour  {

    protected static T Instantiate<T>(string prefabPath) where T : FullScreenMenuView {
        var menu = Instantiate(Resources.Load<GameObject>(prefabPath)).GetComponent<T>();
        menu.transform.SetParent(Global.Instance().UI.transform, worldPositionStays: false);
        return menu;
    }

    public virtual IEnumerator CloseRoutine() {
        Destroy(gameObject);
        yield break; ;
    }
}
