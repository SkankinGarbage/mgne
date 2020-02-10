using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// When the number of cells in the list exceeds a threshold, expand spacing by the expansion amount
/// </summary>
public class PaddingExpanderComponent : MonoBehaviour {

    [SerializeField] private GridLayoutGroup grid = null;
    [SerializeField] private HorizontalOrVerticalLayoutGroup group = null;
    [SerializeField] private ListView list = null;
    [SerializeField] private int threshold = 0;
    [SerializeField] private int belowThresholdSpacing = 0;
    [SerializeField] private int aboveThresholdSpacing = 0;

    public void OnEnable() {
        list.OnPopulate += List_OnPopulate;
    }

    public void OnDisable() {
        list.OnPopulate -= List_OnPopulate;
    }

    private void List_OnPopulate(int dataCount) {
        int spacing = dataCount >= threshold ? aboveThresholdSpacing : belowThresholdSpacing;
        if (grid != null) grid.spacing = new Vector2(grid.spacing.x, spacing);
        if (group != null) group.spacing = spacing;
    }
}
