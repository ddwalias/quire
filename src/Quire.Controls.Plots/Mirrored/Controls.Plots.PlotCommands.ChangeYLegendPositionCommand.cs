namespace Quire.Controls.Plots.PlotCommands;

internal partial class ChangeYLegendPositionCommand : PlotCommands
{
    private readonly int _newPositionIndex;

    private int _oldPositionIndex;

    public ChangeYLegendPositionCommand(PlotControl plot, int newIndex)
        : base(plot, "Change Y Legend Position Command")
    {
        _newPositionIndex = newIndex;
    }

    public override bool Do()
    {
        _oldPositionIndex = _plot.YLegendPositionIndex;
        _plot.YLegendPositionIndex = _newPositionIndex;
        if (_newPositionIndex == _oldPositionIndex)
        {
            return false;
        }
        return true;
    }

    public override void Undo()
    {
        _plot.YLegendPositionIndex = _oldPositionIndex;
    }

    public override void Redo()
    {
        Do();
    }
}
