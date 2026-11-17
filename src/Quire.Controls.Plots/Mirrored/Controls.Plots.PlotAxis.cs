#nullable disable
#pragma warning disable

using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots;

public partial struct PlotAxis
{
    public string StartSExpression { get; set; }

    public string SecondTickSExpression { get; set; }

    public string EndSExpression { get; set; }

    public int PositionTickMark { get; set; }

    public NumberFormat DisplayNumberFormat { get; set; }

    public AxisScaling Scaling { get; set; }

    public bool ShowTicks { get; set; }

    public bool ShowNumbers { get; set; }

    public bool ShowGridLines { get; set; }

    public Color GridLineColor { get; set; }
}
