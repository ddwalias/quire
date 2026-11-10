using Quire.Controls.InputControls;
using System.Collections.Generic;
using System.Linq;

namespace Quire.Controls.InputControls;

public partial class ComboBoxTableModel
{
    public IEnumerable<INameValuesRow> NameValuesRows => _nameValuesRows;

    public bool CanRemoveRow(int rowIndex)
    {
        if (0 < rowIndex && rowIndex < Rows)
        {
            return Rows > 2;
        }

        return false;
    }

    public bool CanRemoveColumn(int columnIndex)
    {
        if (0 < columnIndex && columnIndex < Columns)
        {
            return Columns > 2;
        }

        return false;
    }

    public int Rows => _nameValuesRows.Count + 1;

    public int Columns
    {
        get => _columns;
        private set
        {
            if (DebugTools.DevAssert(value > 0, null).Passed)
            {
                _columns = value;
            }
        }
    }

    public System.Collections.Generic.IEnumerable<Quire.Controls.InputControls.ComboBoxRow> AllRows => _topRow.Yield<ComboBoxRow>().Concat<ComboBoxRow>(_nameValuesRows);

    public System.Collections.Generic.IEnumerable<Quire.EquationEditor.View.IReadonlyCellData> UnitsForValueColumns => _topRow.UnitCells;
}
