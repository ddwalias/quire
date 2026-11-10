namespace Quire.Controls.InputControls;

public partial class ComboBoxViewModelBase
{

    public Quire.EquationEditor.Tree.IComboBoxInputControlDataAdapter CreateDataAdapter()
    {
        return new ComboBoxTableModelAdapter(TableModel);
    }
}
