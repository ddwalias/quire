#nullable disable
#pragma warning disable

using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.View;
using Quire.Undo;
using Quire.Calculation.Core;

namespace Quire.Controls.Plots.ViewModel;

internal partial class AxisMarkerViewModel
{
    private readonly AxisMarker _axisMarker;

    protected AxisMarkerViewModel(AxisMarker axisMarker) : base(axisMarker)
    {
        _axisMarker = axisMarker;
        Calculatable = _axisMarker.Calculatable;
        RealNumberCalculatable.RequestShowError += (object? obj, RequestShowErrorEventArgs args) => Activate();
        CreateCommandBinding(PlotControlBase.ChangeTraceLineCommand, ChangeLineStyle, CanChangeLineStyle);
        CreateCommandBinding(PlotControlBase.ChangeTraceWeightCommand, ChangeLineWeight);
    }

    private static bool CanChangeLineStyle(object parameter)
    {
        if (parameter == null)
        {
            return true;
        }
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(parameter is LineStyle, null))
        {
            return false;
        }
        return (LineStyle)parameter != LineStyle.None;
    }

    private void ChangeLineStyle(object parameter)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(parameter is LineStyle, null))
        {
            return;
        }
        LineStyle = (LineStyle)parameter;
    }

    private void ChangeLineWeight(object parameter)
    {
        if ((AssertionE)DebugTools.DevAssert(parameter is double, null) == AssertionE.Passed)
        {
            LineWeight = (double)parameter;
        }
    }

    public double Value
    {
        get => _axisMarker.Value;
        set
        {
            if (_axisMarker.Value != value)
            {
                ExecuteChangeValueAndIsUserSetFlagCommand("Set Line Marker Value", value, newIsUserSetValue: false);
            }
        }
    }

    public LineStyle LineStyle
    {
        get => _axisMarker.LineStyle;
        set
        {
            if (value != _axisMarker.LineStyle)
            {
                LineStyle oldValue = _axisMarker.LineStyle;
                CommandBuffer.Execute(new UndoableCommand("Set Line Marker LineStyle", () =>
                {
                    _axisMarker.LineStyle = value;
                    return true;
                }, () => _axisMarker.LineStyle = oldValue));
            }
        }
    }

    public double LineWeight
    {
        get => _axisMarker.LineWeight;
        set
        {
            if (value != _axisMarker.LineWeight)
            {
                double oldValue = _axisMarker.LineWeight;
                CommandBuffer.Execute(new UndoableCommand("Set Line Marker LineWeight", () =>
                {
                    _axisMarker.LineWeight = value;
                    return true;
                }, () => _axisMarker.LineWeight = oldValue));
            }
        }
    }

    public IEquationProxy EquationProxy
    {
        get => RealNumberCalculatable.EquationProxy;
        set
        {
            RealNumberCalculatable.EquationProxy = value;
            _axisMarker.SetEquationProxy(value);
        }
    }

    public bool IsUserSetValue
    {
        get => _axisMarker.IsUserSetValue;
        set
        {
            if (IsUserSetValue != value)
            {
                double previousValue = Value;
                ExecuteChangeValueAndIsUserSetFlagCommand("Set Is-User-Set-Value", previousValue, value);
            }
        }
    }

    public RealNumberCalculatable RealNumberCalculatable => _axisMarker.RealNumberCalculatable;

    private void ExecuteChangeValueAndIsUserSetFlagCommand(string commandName, double newValue, bool newIsUserSetValue)
    {
        if (EnvironmentScope.IsCurrentlyInsideUndoOrRedo)
        {
            return;
        }
        bool oldIsUserSetValue = IsUserSetValue;
        double oldValue = Value;
        string oldProxySExpression = IsUserSetValue ? EquationProxy.EquationSExpression : null;
        CommandBuffer.Execute(new UndoableCommandWithSideEffects(commandName, () =>
        {
            InternalSetIsUserSetValueKeepHiddenFromCalculation(newIsUserSetValue);
            InternalSetValue(newValue);
            return true;
        }, () =>
        {
            if (oldIsUserSetValue)
            {
                EquationProxy.EquationSExpression = oldProxySExpression;
            }
            InternalSetIsUserSetValueKeepHiddenFromCalculation(oldIsUserSetValue);
            InternalSetValue(oldValue);
        }));
    }

    private void InternalSetIsUserSetValueKeepHiddenFromCalculation(bool newIsUserSetValue) => _axisMarker.SetIsUserSetValue(newIsUserSetValue, !newIsUserSetValue);

    private void InternalSetValue(double value) => _axisMarker.Value = value;
}
