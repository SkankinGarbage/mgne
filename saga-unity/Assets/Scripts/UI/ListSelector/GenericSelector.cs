using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;

public abstract class GenericSelector : MonoBehaviour {

    protected int selection;

    public async Task<string> SelectCommandAsync(Action<int> scanner = null) {
        var result = await SelectItemAsync(scanner);
        if (result == -1) {
            return null;
        } else {
            return GetCell(result).GetComponent<CommandCell>().CommandString;
        }
    }

    public void ClearSelection() {
        GetCell(selection).SetSelected(false);
    }

    public async Task<int> SelectItemAsync(Action<int> scanner = null, bool leavePointerEnabled = false) {
        var completion = new TaskCompletionSource<int>();

        if (!leavePointerEnabled) selection = 0;
        UpdateSelection(selection);
        while (!GetCell(selection).IsSelectable()) {
            MoveSelectionVertical(1);
        }
        GetCell(selection).SetSelected(true);
        scanner?.Invoke(selection);

        string listenerId = "ListSelector" + gameObject.name;
        Global.Instance().Input.PushListener(listenerId, (InputManager.Command command, InputManager.Event ev) => {
            if (ev != InputManager.Event.Down && ev != InputManager.Event.Repeat) {
                return true;
            }
            switch (command) {
                case InputManager.Command.Cancel:
                    Global.Instance().Input.RemoveListener(listenerId);
                    if (!leavePointerEnabled) TurnOffPointer();
                    completion.SetResult(-1);
                    break;
                case InputManager.Command.Confirm:
                    Global.Instance().Input.RemoveListener(listenerId);
                    if (!leavePointerEnabled) TurnOffPointer();
                    completion.SetResult(selection);
                    break;
                case InputManager.Command.Up:
                    MoveSelectionVertical(-1);
                    scanner?.Invoke(selection);
                    break;
                case InputManager.Command.Down:
                    MoveSelectionVertical(1);
                    scanner?.Invoke(selection);
                    break;
                case InputManager.Command.Left:
                    MoveSelectionHorizontal(-1);
                    scanner?.Invoke(selection);
                    break;
                case InputManager.Command.Right:
                    MoveSelectionHorizontal(1);
                    scanner?.Invoke(selection);
                    break;
            }
            return true;
        });

        return await completion.Task;
    }

    public void TurnOffPointer() {
        foreach (SelectableCell child in GetCells()) {
            child.GetComponent<SelectableCell>().SetSelected(false);
        }
    }

    protected abstract int CellCount();

    protected abstract SelectableCell GetCell(int index);

    protected abstract IEnumerable<SelectableCell> GetCells();

    protected virtual void MoveSelectionHorizontal(int delta) {

    }

    protected virtual void MoveSelectionVertical(int delta) {
        UpdateSelection(delta + selection);
    }

    protected void UpdateSelection(int newSelection) {
        GetCell(selection).SetSelected(false);
        if (newSelection < 0) newSelection = CellCount() - 1;
        if (newSelection >= CellCount()) newSelection = 0;
        GetCell(newSelection).SetSelected(true);
        selection = newSelection;
    }
}
