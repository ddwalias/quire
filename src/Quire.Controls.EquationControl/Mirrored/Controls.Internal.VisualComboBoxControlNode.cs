using Quire.Controls.InputControls;
using Quire.EquationEditor.Tree;
using System.Windows.Controls;
namespace Quire.Controls.Internal;

internal partial class VisualComboBoxControlNode
{

    public void SetSelectedIndex(int index)
    {
        if (!ComboBoxControl.IsEnabled)
        {
            return;
        }

        ComboBoxControl.SetSelectedIndex(index);
    }

    private Quire.Controls.InputControls.ComboBoxControl ComboBoxControl => (ComboBoxControl)_inputControl;

    public bool IsDropDownOpen
    {
        get => ComboBoxControl.IsDropDownOpen;
        set => ComboBoxControl.IsDropDownOpen = value;
    }

    public Quire.EquationEditor.Tree.IComboBoxControlNodeInfo ComboBoxControlInfo => NodeInfo as IComboBoxControlNodeInfo;

    public string GetSelectedDisplayedText => ComboBoxControl.SelectedItemValue;

    public bool IsEditModeActive => ComboBoxControl.IsEditModeActive;

    public Quire.EquationEditor.View.IEditableComboBoxVisualTable EditableComboBoxTable => ComboBoxControl.EditableComboBoxVisualTable;
}
