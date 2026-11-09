namespace Quire.Controls.InputControls;

public partial interface INameValuesRow
{
    global::Quire.EquationEditor.View.IReadonlyCellData NameCell { get; }
    global::System.Collections.Generic.IEnumerable<global::Quire.EquationEditor.View.IReadonlyCellData> ValueCells { get; }
    string DisplayName { get; set; }
}
