using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextScrollView : MonoBehaviour {

    [SerializeField] private Text textArea = null;
    [SerializeField] private int scrollDistance = 480;
    [SerializeField] private float scrollSeconds = 20;

    public IEnumerator ScrollRoutine() {
        float elapsed = 0;
        float @base = textArea.rectTransform.anchoredPosition.y;
        while (elapsed < scrollSeconds) {
            elapsed += Time.deltaTime * (Global.Instance().Input.IsFastKeyDown() ? 7 : 1);
            float offset = elapsed / scrollSeconds * scrollDistance;
            textArea.rectTransform.anchoredPosition = new Vector2(textArea.rectTransform.anchoredPosition.x, @base + offset);
            yield return null;
        }
    }
}
