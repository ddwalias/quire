using System.Windows;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.View;

namespace Quire.Controls.Plots;

public partial class PolarPlotControl
{
    static PolarPlotControl()
    {
        PlotControlBase.HasOriginPositioningProperty.OverrideMetadata(typeof(PolarPlotControl), new PropertyMetadata(false));
    }

    public PolarPlotControl()
        : base(PlotType.Polar, new PolarPlotStrategy())
    {
        Graph.AngularAxis = RangeAxis as PlotGraphAxis;
        Graph.RadialAxis = PrimaryResultAxis as PlotGraphAxis;
    }

    public override void SetGraphSizeForXamlSerialization()
    {
        SetGraphSize(0);
    }
}
