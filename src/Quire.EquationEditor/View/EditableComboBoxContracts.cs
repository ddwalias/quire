namespace Quire.EquationEditor.View
{
    public partial interface IEditableComboBoxViewModel
    {
        int SelectedTableRowIndex { get; }

        int CellCount { get; }

        int Rows { get; }

        int Columns { get; }

        global::System.Windows.Size PlaceHolderBoxSize { get; }

        double MidPointBetweenFirstAndSecondRow { get; }

        double MidPointBetweenFirstAndSecondColumn { get; }

        global::System.Windows.Size TableSize { get; }

        IReadonlyCellData GetCellAt(int row, int column);

        void CreateSelection(int firstCellIdx, int secondCellIdx);
    }

    public partial interface IEditableComboBoxVisualTable
    {
        IEditableComboBoxViewModel ViewModel { get; }

        bool IsActive { get; }

        global::System.Tuple<int, int> SelectedCells { get; }

        global::Quire.Controls.Core.IMathFormat GetCellFormatAt(int row, int column);
    }

    public partial interface IReadonlyCellData
    {
        string Text { get; }

        int Row { get; }

        int Column { get; }

        bool IsEmpty();

        bool IsEmptyOrWhitespace();
    }
}
