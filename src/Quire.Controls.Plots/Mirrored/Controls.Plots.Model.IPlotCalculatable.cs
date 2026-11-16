using System;

namespace Quire.Controls.Plots.Model;

public partial interface IPlotCalculatable
{
    event EventHandler<EngineErrorEventArgs> PlotCalculatableError;
}
