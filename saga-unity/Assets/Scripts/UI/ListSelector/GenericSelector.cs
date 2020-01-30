using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

public abstract class GenericSelector : MonoBehaviour {

    protected int selection;

    public async Task<string> SelectCommandAsync() {
        var result = await SelectItemAsync();
        if (result == -1) {
            return null;
        } else {
            return GetCell(result).GetComponent<CommandCell>().CommandString;
        }
    }

    public void ClearSelection() {
        GetCell(selection).SetSelected(false);
    }

    public async Task<int> SelectItemAsync() {
        var completion = new TaskCompletionSource<int>();

        selection = 0;
        while (!GetCell(selection).IsSelectable()) {
            MoveSelectionVertical(1);
        }
        GetCell(selection).SetSelected(true);

        string listenerId = "ListSelector" + gameObject.name;
        Global.Instance().Input.PushListener(listenerId, (InputManager.Command command, InputManager.Event ev) => {
            if (ev != InputManager.Event.Down && ev != InputManager.Event.Repeat) {
                return true;
            }
            switch (command) {
                case InputManager.Command.Cancel:
                    completion.SetResult(-1);
                    Global.Instance().Input.RemoveListener(listenerId);
                    break;
                case InputManager.Command.Confirm:
                    completion.SetResult(selection);
                    Global.Instance().Input.RemoveListener(listenerId);
                    break;
                case InputManager.Command.Up:
                    MoveSelectionVertical(-1);
                    break;
                case InputManager.Command.Down:
                    MoveSelectionVertical(1);
                    break;
                case InputManager.Command.Left:
                    MoveSelectionHorizontal(-1);
                    break;
                case InputManager.Command.Right:
                    MoveSelectionHorizontal(1);
                    break;
            }
            return true;
        });

        await completion.Task;

        DisableSelection();

        return await completion.Task;
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

    private void DisableSelection() {
        foreach (SelectableCell child in GetCells()) {
            child.GetComponent<SelectableCell>().SetSelected(false);
        }
    }
}
