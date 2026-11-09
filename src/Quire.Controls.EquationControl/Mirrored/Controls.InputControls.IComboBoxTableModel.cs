namespace Quire.Controls.InputControls;

public partial interface IComboBoxTableModel
{
    string TableTitle { get; }
    int SelectedNameValuesRowIndex { get; }
    global::System.Collections.Generic.IEnumerable<global::Quire.EquationEditor.View.IReadonlyCellData> UnitsForValueColumns { get; }
    global::System.Collections.Generic.IEnumerable<global::Quire.Controls.InputControls.INameValuesRow> NameValuesRows { get; }
    global::Quire.EquationEditor.View.IReadonlyCellData GetCellAt(int rowIdx, int colIdx);
}
