using System.Collections.Generic;
using System.Windows.Controls;
using Quire.Controls.Core;

namespace Quire.Controls.Plots;

internal partial class PlotLegendLayoutManager
{
    public PlotLegendLayoutManager()
    {
    }

    private PlotLegendLayoutManager(PlotLegendLayoutManager other)
        : base(other)
    {
    }

    protected override MaxMatrixWidth[] CalcMaxDecimalPerColumn(IList<LayoutMetrics> cellsInfo, UIElementCollection cellsContent, int rows)
    {
        return new MaxMatrixWidth[cellsContent.Count];
    }

    public override object Clone()
    {
        return new PlotLegendLayoutManager(this);
    }
}
