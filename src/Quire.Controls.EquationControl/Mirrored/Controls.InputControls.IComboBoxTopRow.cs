using System.Collections.Generic;
using Quire.EquationEditor.View;

namespace Quire.Controls.InputControls;

internal partial interface IComboBoxTopRow
{
    IReadonlyCellData TableTitleCell { get; }

    IEnumerable<IReadonlyCellData> UnitCells { get; }
}
