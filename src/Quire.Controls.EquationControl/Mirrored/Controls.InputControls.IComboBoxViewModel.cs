namespace Quire.Controls.InputControls;

public partial interface IComboBoxViewModel
{
    global::System.Collections.Generic.IEnumerable<global::Quire.Controls.InputControls.INameValuesRow> DataItems { get; }
    string TableTitle { get; }
    int SelectedRowIndex { get; }
    bool RefreshInProgress { get; }
    void LoadFrom(global::Quire.EquationEditor.Tree.IComboBoxInputControlDataAdapter data);
    global::Quire.EquationEditor.Tree.IComboBoxInputControlDataAdapter CreateDataAdapter();
}
