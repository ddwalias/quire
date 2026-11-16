namespace Quire.Controls.Plots.Plot3D.ViewModel;

internal partial interface IPlotEquationViewModel3D
{
    Model.IPlotTraceModel3D TraceModel { get; }
    Presenter.IPlotEquation EquationProxy { get; set; }
}
