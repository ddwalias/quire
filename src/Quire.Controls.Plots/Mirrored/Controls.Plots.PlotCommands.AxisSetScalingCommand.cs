using System.Windows;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;
using Quire.Wpf;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class AxisSetScalingCommand : AxisCommand
{
    private readonly AxisScaling _oldScaling;

    private readonly AxisScaling _newScaling;

    private bool _changedNumberStyle;

    private PlotControl _parentPlot;

    public AxisSetScalingCommand(PlotGraphAxis axis, AxisScaling axesScaling)
        : base(axis, "Axis Set Scaling Command")
    {
        _oldScaling = _axis.Scaling;
        _newScaling = axesScaling;
        _parentPlot = _axis.TryFindVisualAncestor<PlotControl>();
        DebugTools.DevAssert(_parentPlot.ActiveAxis.Equals(_axis), "Scaling can be set on active axis only!");
    }

    public override bool Do()
    {
        bool flag = false;
        if (_axis.Scaling != _newScaling)
        {
            _axis.Scaling = _newScaling;
            flag = (_axis.Scaling == _newScaling);
            if (flag && _newScaling == AxisScaling.Logarithmic && !_axis.HasLocalValue(PlotGraphAxis.DisplayNumberFormatProperty) && _axis.DisplayNumberFormat != null)
            {
                _changedNumberStyle = true;
                _axis.DisplayNumberFormat = _axis.DisplayNumberFormat.Clone();
                _axis.DisplayNumberFormat.NumberStyle = NumberFormat.NumberStyles.Scientific;
            }
        }
        return flag;
    }

    public override void Undo()
    {
        _axis.Scaling = _oldScaling;
        if (_changedNumberStyle)
        {
            _axis.ClearValue(EquationControl.DisplayNumberFormatProperty);
        }
    }

    public override void Redo()
    {
        Do();
    }
}
