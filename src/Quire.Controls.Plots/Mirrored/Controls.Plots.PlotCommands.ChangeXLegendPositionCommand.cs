namespace Quire.Controls.Plots.PlotCommands;

internal partial class ChangeXLegendPositionCommand : PlotCommands
{
    private readonly int _newPositionIndex;

    private int _oldPositionIndex;

    public ChangeXLegendPositionCommand(PlotControl plot, int newIndex)
        : base(plot, "Change X Legend Position Command")
    {
        _newPositionIndex = newIndex;
    }

    public override bool Do()
    {
        _oldPositionIndex = _plot.XLegendPositionIndex;
        _plot.XLegendPositionIndex = _newPositionIndex;
        if (_newPositionIndex == _oldPositionIndex)
        {
            return false;
        }
        return true;
    }

    public override void Undo()
    {
        _plot.XLegendPositionIndex = _oldPositionIndex;
    }

    public override void Redo()
    {
        Do();
    }
}
