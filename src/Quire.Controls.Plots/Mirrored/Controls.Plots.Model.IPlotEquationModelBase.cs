namespace Quire.Controls.Plots.Model;

public partial interface IPlotEquationModelBase
{
    Calculation.Core.EngineError PlotCalculatableError { get; set; }
}
