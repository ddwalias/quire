using Quire.Controls.Core;

namespace Quire.Controls.Plots;

public partial class PlotLegendContents : VisualTableContents
{
    internal const int DefaultColumns = 1;
    internal const double DefaultColumnSpacing = 8.0;
    internal const double DefaultRowSpacing = 1.0;

    public PlotLegendContents()
        : base(new PlotLegendLayoutManager())
    {
        Columns = 1;
        ColumnSpacing = 8.0;
        RowSpacing = 1.0;
    }

    public int AdjustNumberOfColumnsToWidth(double newWidth)
    {
        return ((PlotLegendLayoutManager)_layoutManager).GetNumberOfColumnsFitWidth(newWidth, Children, ColumnAlignment);
    }
}
