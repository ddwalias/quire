#nullable disable
#pragma warning disable

using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.View;

namespace Quire.Controls.Plots.ViewModel;

internal partial class AxisMarkerViewModel
{
    public double Value
    {
        get
        {
            return _axisMarker.Value;
        }
        set
        {
            if (_axisMarker.Value != value)
            {
                ExecuteChangeValueAndIsUserSetFlagCommand("Set Line Marker Value", value, newIsUserSetValue: false);
            }
        }
    }

    public IEquationProxy EquationProxy
    {
        get
        {
            return RealNumberCalculatable.EquationProxy;
        }
        set
        {
            RealNumberCalculatable.EquationProxy = value;
            _axisMarker.SetEquationProxy(value);
        }
    }

    public bool IsUserSetValue
    {
        get
        {
            return _axisMarker.IsUserSetValue;
        }
        set
        {
            if (IsUserSetValue != value)
            {
                double value2 = Value;
                ExecuteChangeValueAndIsUserSetFlagCommand("Set Is-User-Set-Value", value2, value);
            }
        }
    }

    public RealNumberCalculatable RealNumberCalculatable => _axisMarker.RealNumberCalculatable;

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

    private void ChangeLineWeight(object parameter)
    {
        if ((AssertionE)DebugTools.DevAssert(parameter is double, null) == AssertionE.Passed)
        {
            LineWeight = (double)parameter;
        }
    }

    private void InternalSetIsUserSetValueKeepHiddenFromCalculation(bool newIsUserSetValue)
    {
        _axisMarker.SetIsUserSetValue(newIsUserSetValue, !newIsUserSetValue);
    }

    private void InternalSetValue(double value)
    {
        _axisMarker.Value = value;
    }
}
