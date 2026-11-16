using System;
using System.ComponentModel;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Plots.Plot3D.Model;

public partial interface IPlotEquationModel3D : IPlotEquationModelBase, INotifyPropertyChanged
{
    bool IsCommitted { get; }

    IEquationControl Expression { get; }

    IPlotEquation EquationProxy { get; set; }

    IPlotTraceModel3D Trace { get; set; }

    event Action RequestEvaluation;

    event Action EquationProxyChanged;

    event Action EquationProxyChanging;

    void Commit();

    void SetDataObject(plotEquation value);

    plotEquation GetDataObject();
}
