using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;

public abstract class GenericSelector : MonoBehaviour {

    private string ListenerId => "ListSelector" + gameObject.name;

    private int selection;
    public int Selection {
        set {
            int oldSelection = selection;
            GetCell(selection).SetSelected(false);
            if (value < 0) value = CellCount() - 1;
            if (value >= CellCount()) value = 0;
            GetCell(value).SetSelected(true);
            selection = value;
        }
        get => selection;
    }

    public async Task<string> SelectCommandAsync(Action<int> scanner = null) {
        var result = await SelectItemAsync(scanner);
        if (result == -1) {
            return null;
        } else {
            return GetCell(result).GetComponent<CommandCell>().CommandString;
        }
    }

    public void ClearSelection() {
        GetCell(Selection).SetSelected(false);
    }

    public async Task<int> SelectItemAsync(Action<int> scanner = null, bool leavePointerEnabled = false) {
        var completion = new TaskCompletionSource<int>();

        if (!leavePointerEnabled) Selection = 0;
        while (!GetCell(Selection).IsSelectable()) {
            MoveSelectionVertical(1);
        }
        Selection = Selection;
        scanner?.Invoke(Selection);

        bool canceled = false; 
        Global.Instance().Input.PushListener(ListenerId, (InputManager.Command command, InputManager.Event ev) => {
            if (ev != InputManager.Event.Down) {
                return true;
            }
            if (canceled) {
                return true;
            }
            switch (command) {
                case InputManager.Command.Cancel:
                    Global.Instance().Input.RemoveListener(ListenerId);
                    canceled = true;
                    if (!leavePointerEnabled) TurnOffPointer();
                    completion.SetResult(-1);
                    break;
                case InputManager.Command.Confirm:
                    Global.Instance().Input.RemoveListener(ListenerId);
                    if (!leavePointerEnabled) TurnOffPointer();
                    completion.SetResult(Selection);
                    break;
                case InputManager.Command.Up:
                    MoveSelectionVertical(-1);
                    scanner?.Invoke(Selection);
                    break;
                case InputManager.Command.Down:
                    MoveSelectionVertical(1);
                    scanner?.Invoke(Selection);
                    break;
                case InputManager.Command.Left:
                    MoveSelectionHorizontal(-1);
                    scanner?.Invoke(Selection);
                    break;
                case InputManager.Command.Right:
                    MoveSelectionHorizontal(1);
                    scanner?.Invoke(Selection);
                    break;
            }
            return true;
        });

        return await completion.Task;
    }

    public async Task<bool> ConfirmSelectionAsync(bool clearSelectionWhenDone = true) {
        var completion = new TaskCompletionSource<bool>();
        bool canceled = false;
        Global.Instance().Input.PushListener(ListenerId, (InputManager.Command command, InputManager.Event ev) => {
            if (ev != InputManager.Event.Down && ev != InputManager.Event.Repeat) {
                return true;
            }
            if (canceled) {
                return true;
            }
            switch (command) {
                case InputManager.Command.Cancel:
                    Global.Instance().Input.RemoveListener(ListenerId);
                    canceled = true;
                    TurnOffPointer();
                    completion.SetResult(false);
                    break;
                case InputManager.Command.Confirm:
                    Global.Instance().Input.RemoveListener(ListenerId);
                    TurnOffPointer();
                    completion.SetResult(true);
                    break;
            }
            return true;
        });

        var result = await completion.Task;
        if (clearSelectionWhenDone) {
            ClearSelection();
        }
        return result;
    }

    public void TurnOffPointer() {
        foreach (SelectableCell child in GetCells()) {
            child.GetComponent<SelectableCell>().SetSelected(false);
        }
    }

    public void SelectAll() {
        foreach (var cell in GetCells()) {
            cell.SetSelected(true);
        }
    }

    protected abstract int CellCount();

    protected abstract SelectableCell GetCell(int index);

    protected abstract IEnumerable<SelectableCell> GetCells();

    protected virtual void MoveSelectionHorizontal(int delta) {

    }

    protected virtual void MoveSelectionVertical(int delta) {
        Selection = delta + Selection;
    }
}
