#nullable disable
#pragma warning disable

using Quire.Controls.Core;
using Quire.Controls.Plots.Base3D.ViewModel;

namespace Quire.Controls.Plots.Plot3D.View;

internal partial class PlotAxesWidget3DViewModel
{
    public Base3DAxisViewModel XAxisViewModel { get; private set; }

    public Base3DAxisViewModel YAxisViewModel { get; private set; }

    public Base3DAxisViewModel ZAxisViewModel { get; private set; }

    public PlotAxesWidget3DViewModel(Base3DAxisViewModel xAxisViewModel, Base3DAxisViewModel yAxisViewModel, Base3DAxisViewModel zAxisViewModel)
    {
        XAxisViewModel = xAxisViewModel;
        YAxisViewModel = yAxisViewModel;
        ZAxisViewModel = zAxisViewModel;
    }
}
