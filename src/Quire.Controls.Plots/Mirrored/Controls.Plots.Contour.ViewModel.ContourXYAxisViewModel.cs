using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Contour.ViewModel;

internal partial class ContourXYAxisViewModel
{
    internal ContourXYAxisViewModel(AxesType axisType)
        : base(axisType, PlotType.Contour)
    {
    }

    internal override void SetResultRange(double? resultStart, double? resultEnd)
    {
        ResultStart = resultStart;
        ResultEnd = resultEnd;
        LimitsExpressionValueChanged();
    }
}
