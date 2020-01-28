using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Generic list controller. Needs testing.
/// </summary>
public class DynamicListSelector : ListSelector {

    private float HideShowDuration = 0.4f;

    public GameObject childAttachPoint;
    public int selection { get; private set; }

    public void Awake() {
        DestroyAllChildren();
    }

    public List<SelectableCell> PopulateCells<T>(List<T> elements, Func<T, SelectableCell> cellConstructor) {
        DestroyAllChildren();
        List<SelectableCell> allCells = new List<SelectableCell>();
        foreach (T element in elements) {
            SelectableCell cell = cellConstructor(element);
            cell.SetSelected(false);
            cell.transform.SetParent(childAttachPoint.transform);
            allCells.Add(cell);
        }
        return allCells;
    }

    public IEnumerator SelectAndCloseRoutine<T>(Result<T> result, List<T> elements, Func<T, SelectableCell> cellConstructor) {
        yield return SelectAndPersistRoutine(result, elements, cellConstructor);
        yield return ShowHideRoutine(true);
        gameObject.SetActive(false);
    }

    public IEnumerator SelectAndPersistRoutine<T>(Result<T> result, List<T> elements, Func<T, SelectableCell> cellConstructor) {
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            yield return ShowHideRoutine();
        }
        List<SelectableCell> cells = PopulateCells(elements, cellConstructor);
        Result<int> subResult = new Result<int>();
        yield return RawSelectRoutine(subResult);
        if (subResult.canceled) {
            yield return ShowHideRoutine(true);
            result.Cancel();
        } else {
            result.value = elements[subResult.value];
        }
    }

    public IEnumerator RawSelectRoutine(Result<int> result) {
        yield return null;
        while (!GetCell(selection).IsSelectable()) {
            MoveSelection(1);
        }
        GetCell(selection).SetSelected(true);

        string listenerId = "ListSelector" + gameObject.name;
        Global.Instance().Input.PushListener(listenerId, (InputManager.Command command, InputManager.Event ev) => {
            if (ev != InputManager.Event.Up && ev != InputManager.Event.Repeat) {
                return true;
            }
            switch (command) {
                case InputManager.Command.Cancel:
                    result.Cancel();
                    Global.Instance().Input.RemoveListener(listenerId);
                    break;
                case InputManager.Command.Confirm:
                    result.value = selection;
                    Global.Instance().Input.RemoveListener(listenerId);
                    break;
                case InputManager.Command.Up:
                    MoveSelection(-1);
                    break;
                case InputManager.Command.Down:
                    MoveSelection(1);
                    break;
            }
            return true;
        });

        while (!result.finished) {
            yield return null;
        }
        DisableSelection();
    }

    public IEnumerator ShowHideRoutine(bool hide = false) {
        if (hide && !gameObject.activeSelf) {
            yield break;
        }

        GetComponent<ContentSizeFitter>().enabled = false;
        CanvasGroup group = childAttachPoint.GetComponent<CanvasGroup>();
        RectTransform rect = GetComponent<RectTransform>();

        float endHeight = hide ? 0.0f : LayoutUtility.GetPreferredHeight(rect);
        Vector2 endSize = new Vector2(rect.sizeDelta.x, endHeight);

        if (!hide) {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 0.0f);
            group.alpha = 0.0f;
        } else {
            selection = 0;
        }
        
        var expandTween = rect.DOSizeDelta(endSize, HideShowDuration / 2.0f);
        var fadeTween = group.DOFade(hide ? 0.0f : 1.0f, HideShowDuration / 2.0f);
        fadeTween.SetEase(Ease.Linear);

        if (hide) {
            yield return CoUtils.RunTween(fadeTween);
            yield return CoUtils.RunTween(expandTween);
        } else {
            yield return CoUtils.RunTween(expandTween);
            yield return CoUtils.RunTween(fadeTween);
        }

        gameObject.SetActive(!hide);
        GetComponent<ContentSizeFitter>().enabled = true;
    }

    private void DestroyAllChildren() {
        foreach (Transform child in childAttachPoint.transform) {
            Destroy(child.gameObject);
        }
    }

    private void MoveSelection(int delta) {
        GetCell(selection).SetSelected(false);
        int newSelection = selection + delta;
        if (newSelection < 0) newSelection = childAttachPoint.transform.childCount - 1;
        if (newSelection >= childAttachPoint.transform.childCount) newSelection = 0;
        GetCell(newSelection).SetSelected(true);
        selection = newSelection;
    }

    private void DisableSelection() {
        foreach (Transform child in childAttachPoint.transform) {
            child.GetComponent<SelectableCell>().SetSelected(false);
        }
    }

    private SelectableCell GetCell(int index) {
        return childAttachPoint.transform.GetChild(index).GetComponent<SelectableCell>();
    }
}
