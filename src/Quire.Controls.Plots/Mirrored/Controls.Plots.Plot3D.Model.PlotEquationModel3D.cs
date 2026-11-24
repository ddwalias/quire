using System;
using System.ComponentModel;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Plots.Plot3D.Model;

internal partial class PlotEquationModel3D : PlotEquationModelBase, IPlotEquationModel3D, IPlotEquationModelBase, INotifyPropertyChanged
{
    private partial class DoActionWhenEquationProxyChanged
    {
        private readonly Action _action;
        private readonly PlotEquationModel3D _plotEquationModel;

        public DoActionWhenEquationProxyChanged(PlotEquationModel3D plotEquationModel, Action action)
        {
            _action = action;
            _plotEquationModel = plotEquationModel;
            if (_plotEquationModel._equationProxy != null)
            {
                _action();
            }
            else
            {
                _plotEquationModel.EquationProxyChanged += ActionWrapper;
            }
        }

        private void ActionWrapper()
        {
            if (_plotEquationModel._equationProxy != null)
            {
                _plotEquationModel.EquationProxyChanged -= ActionWrapper;
                _action();
            }
        }
    }

    private IPlotEquation _equationProxy;

    public event Action EquationProxyChanging;

    private void InvokeEquationProxyChanging()
    {
        // IL requires an explicit local to emit stloc.0 rather than dup.
        Action handler = EquationProxyChanging;
        if (handler != null)
        {
            handler();
        }
    }

    public event Action EquationProxyChanged;

    private void InvokeEquationProxyChanged()
    {
        // IL requires an explicit local to emit stloc.0 rather than dup.
        Action handler = EquationProxyChanged;
        if (handler != null)
        {
            handler();
        }
    }

    public IPlotTraceModel3D Trace { get; set; }

    public IPlotEquation EquationProxy
    {
        set
        {
            if (_equationProxy != value)
            {
                InvokeEquationProxyChanging();
                if (_equationProxy != null)
                {
                    _equationProxy.RequestEvaluation -= FireRequestEvaluation;
                }
                _equationProxy = value;
                if (DebugTools.AssertNotNull(_equationProxy, null).Passed)
                {
                    _equationProxy.RequestEvaluation += FireRequestEvaluation;
                }
                InvokeEquationProxyChanged();
            }
        }
        get
        {
            return _equationProxy;
        }
    }

    public event Action RequestEvaluation;

    public void Commit()
    {
        if (DebugTools.AssertNotNull(_equationProxy, null).Passed)
        {
            _equationProxy.Commit();
        }
    }

    public bool IsCommitted => _equationProxy.IsCommitted;

    public IEquationControl Expression => _equationProxy.Expression;

    public void SetDataObject(plotEquation value)
    {
        new DoActionWhenEquationProxyChanged(this, () =>
        {
            _equationProxy.SetDataObject(value);
            if (Trace != null && Trace.SynchronizationStatus != SynchronizationStatus.Synchronized)
            {
                _equationProxy.Commit();
            }
        });
    }

    public plotEquation GetDataObject()
    {
        return _equationProxy.GetDataObject();
    }

    private void FireRequestEvaluation()
    {
        // IL requires an explicit local to emit stloc.0 rather than dup.
        Action handler = RequestEvaluation;
        if (handler != null)
        {
            handler();
        }
    }

    public PlotEquationModel3D()
    {
    }
}
