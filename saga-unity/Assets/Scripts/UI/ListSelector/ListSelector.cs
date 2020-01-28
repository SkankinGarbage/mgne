using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

public class ListSelector : MonoBehaviour {

    public List<SelectableCell> cells;

    private int selection;

    public async Task<string> SelectCommandAsync() {
        var task = SelectItemAsync();
        var result = await task;
        if (task.IsCanceled) return await Task.FromCanceled<string>(new CancellationToken(true));
        return GetCell(result).GetComponent<CommandCell>().CommandString;
    }

    public async Task<int> SelectItemAsync() {
        var completion = new TaskCompletionSource<int>();
        
        selection = 0;
        while (!GetCell(selection).IsSelectable()) {
            MoveSelection(1);
        }
        GetCell(selection).SetSelected(true);

        string listenerId = "ListSelector" + gameObject.name;
        Global.Instance().Input.PushListener(listenerId, (InputManager.Command command, InputManager.Event ev) => {
            if (ev != InputManager.Event.Down && ev != InputManager.Event.Repeat) {
                return true;
            }
            switch (command) {
                case InputManager.Command.Cancel:
                    completion.SetCanceled();
                    Global.Instance().Input.RemoveListener(listenerId);
                    break;
                case InputManager.Command.Confirm:
                    completion.SetResult(selection);
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

        await completion.Task;

        DisableSelection();

        return await completion.Task;
    }

    private void MoveSelection(int delta) {
        GetCell(selection).SetSelected(false);
        int newSelection = selection + delta;
        if (newSelection < 0) newSelection = cells.Count - 1;
        if (newSelection >= cells.Count) newSelection = 0;
        GetCell(newSelection).SetSelected(true);
        selection = newSelection;
    }

    private void DisableSelection() {
        foreach (SelectableCell child in cells) {
            child.GetComponent<SelectableCell>().SetSelected(false);
        }
    }

    private SelectableCell GetCell(int index) {
        return cells[index];
    }
}
