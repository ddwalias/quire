namespace Quire.Controls.Plots.Plot3D.View;

internal partial interface IPlotAxis3D
{
    string[] Labels { get; }
    Base3D.ViewModel.IPlotAxisViewModel3D ViewModel { set; }
}
