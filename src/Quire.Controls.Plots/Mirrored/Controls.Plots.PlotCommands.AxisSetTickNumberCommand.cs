namespace Quire.Controls.Plots.PlotCommands;

internal partial class AxisSetTickNumberCommand : AxisCommand
{
    private readonly int _oldTickNumber;

    private readonly int _newTickNumber;

    private readonly bool _hasOriginPositioningOld;

    private readonly bool _hasOriginPositioningNew;

    public AxisSetTickNumberCommand(PlotGraphAxis axis, int newTickNumber, bool hasOriginPositioningNew)
        : base(axis, "Set Axis Tick Number Command")
    {
        _oldTickNumber = axis.PositionTickMark;
        _newTickNumber = newTickNumber;
        _hasOriginPositioningOld = _axis.HasOriginPositioning;
        _hasOriginPositioningNew = hasOriginPositioningNew;
    }

    public override bool Do()
    {
        _axis.HasOriginPositioning = _hasOriginPositioningNew;
        _axis.PositionTickMark = _newTickNumber;
        return true;
    }

    public override void Undo()
    {
        _axis.HasOriginPositioning = _hasOriginPositioningOld;
        _axis.PositionTickMark = _oldTickNumber;
    }

    public override void Redo()
    {
        Do();
    }
}
