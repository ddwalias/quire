using System;
using System.ComponentModel;
using Quire.Controls.Plots.Plot3D.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;

namespace Quire.Controls.Plots.Plot3D.ViewModel;

internal partial class PlotEquationViewModel3D : PlotEquationViewModelBase<IPlotEquationModel3D>, IPlotEquationViewModel3D, IPlotEquationViewModelBase, INotifyPropertyChanged
{
    public PlotEquationViewModel3D(IPlotEquationModel3D equationModel)
        : base(equationModel)
    {
    }

    public IPlotTraceModel3D TraceModel => Model.Trace;

    [Obsolete]
    public IPlotEquation EquationProxy
    {
        set
        {
            Model.EquationProxy = value;
        }
        get
        {
            return Model.EquationProxy;
        }
    }
}
