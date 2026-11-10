using Quire.Controls.InputControls;
using System.Collections.Generic;

namespace Quire.Controls.InputControls;

public partial class ComboBoxTableModel
{
    public IEnumerable<INameValuesRow> NameValuesRows => _nameValuesRows;
}
