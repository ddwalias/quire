using Quire.Controls.Core;
using Quire.Controls.InputControls;
using Quire.EquationEditor.View;

namespace Quire.Controls.InputControls;

public partial class EditableComboBoxVisualTable
{
    public IEditableComboBoxViewModel ViewModel => _viewModel;

    public ExtendedFont OperatorFormat
    {
        get;
        private set;
    }

    private EditableComboBoxTableCellsFontFormats CellsFontFormats
    {
        get;
        set;
    }
}
