using System.Collections.Generic;

public class ListSelector : GenericSelector {

    public List<SelectableCell> cells;
    public bool horizontal;

    public override SelectableCell GetCell(int index) {
        return cells[index];
    }

    protected override IEnumerable<SelectableCell> GetCells() {
        return cells;
    }

    protected override int CellCount() {
        return cells.Count;
    }

    protected override void MoveSelectionHorizontal(int delta) {
        if (horizontal) {
            base.MoveSelectionVertical(delta);
        }
    }

    protected override void MoveSelectionVertical(int delta) {
        if (!horizontal) {
            base.MoveSelectionVertical(delta);
        }
    }
}
