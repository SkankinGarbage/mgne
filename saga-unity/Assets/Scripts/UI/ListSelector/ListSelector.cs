using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

public class ListSelector : GenericSelector {

    public List<SelectableCell> cells;

    protected override SelectableCell GetCell(int index) {
        return cells[index];
    }

    protected override IEnumerable<SelectableCell> GetCells() {
        return cells;
    }

    protected override int CellCount() {
        return cells.Count;
    }
}
