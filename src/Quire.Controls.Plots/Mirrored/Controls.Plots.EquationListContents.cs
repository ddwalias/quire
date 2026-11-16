namespace Quire.Controls.Plots;

internal partial class EquationListContents
{
    public EquationListContents()
        : base(new EquationListLayoutManager())
    {
        Columns = 1;
        ColumnSpacing = 8.0;
        RowSpacing = 1.0;
        ColumnAlignment = Core.ColumnAlignments.CenterAlign;
    }

    public int AdjustNumberOfColumnsToWidth(double newWidth)
    {
        return ((EquationListLayoutManager)_layoutManager).GetNumberOfColumnsFitWidth(newWidth, Children, ColumnAlignment);
    }
}
