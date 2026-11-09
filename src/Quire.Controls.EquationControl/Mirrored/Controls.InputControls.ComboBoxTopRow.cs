using System.Collections.Generic;
using Quire.EquationEditor.View;

namespace Quire.Controls.InputControls;

public partial class ComboBoxTopRow
{
    public ComboBoxTopRow(int count)
        : base(0, count)
    {
    }

    public ICellData TableTitleCell => FirstCell;

    public IEnumerable<ICellData> UnitCells => AllButFirstCell;

    IReadonlyCellData IComboBoxTopRow.TableTitleCell => TableTitleCell;

    IEnumerable<IReadonlyCellData> IComboBoxTopRow.UnitCells => UnitCells;
}
