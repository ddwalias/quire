#nullable disable
#pragma warning disable

using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Plot3D.ViewModel;

internal partial class Plot3DAxisViewModel
{
    internal Plot3DAxisViewModel(AxesType axisType)
        : base(axisType, PlotType.Plot3D)
    {
    }

    protected override void LimitsExpressionValueChanged()
    {
    }
}
