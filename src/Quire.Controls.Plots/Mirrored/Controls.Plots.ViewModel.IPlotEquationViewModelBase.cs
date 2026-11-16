namespace Quire.Controls.Plots.ViewModel;

public partial interface IPlotEquationViewModelBase
{
    Calculation.Core.EngineError PlotCalculatableError { get; set; }
}
