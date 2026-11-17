#nullable disable
#pragma warning disable

using Quire.Controls.Plots.ViewModel;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Plots.Contour.View;

public partial struct ContourPlotState
{
    public plotEquation ZEquationDataObject { get; set; }

    public PlotAxis XAxis { get; set; }

    public PlotAxis YAxis { get; set; }

    public PlotAxis ZAxis { get; set; }

    public AxisRangeForXamlSerialization ZAxisRange { get; set; }

    public contourPlotTypeContour contourState { get; set; }

    public bool HasOriginPositioning { get; set; }

    public bool ShowAxisExpressions { get; set; }

    public SizeType PlotGraphSize { get; set; }
}
