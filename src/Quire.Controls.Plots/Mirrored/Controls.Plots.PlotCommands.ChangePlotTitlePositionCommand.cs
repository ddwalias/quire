namespace Quire.Controls.Plots.PlotCommands;

internal partial class ChangePlotTitlePositionCommand : PlotCommands
{
    private readonly int _newPositionIndex;

    private int _oldPositionIndex;

    public ChangePlotTitlePositionCommand(PlotControl plot, int newIndex)
        : base(plot, "Change Plot Title Position Command")
    {
        _newPositionIndex = newIndex;
    }

    public override bool Do()
    {
        _oldPositionIndex = _plot.PlotTitlePositionIndex;
        _plot.PlotTitlePositionIndex = _newPositionIndex;
        if (_newPositionIndex == _oldPositionIndex)
        {
            return false;
        }
        return true;
    }

    public override void Undo()
    {
        _plot.PlotTitlePositionIndex = _oldPositionIndex;
    }

    public override void Redo()
    {
        Do();
    }
}
