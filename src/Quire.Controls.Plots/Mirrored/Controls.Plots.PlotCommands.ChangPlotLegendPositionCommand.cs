namespace Quire.Controls.Plots.PlotCommands;

internal partial class ChangPlotLegendPositionCommand : PlotCommands
{
    private readonly int _newPositionIndex;

    private int _oldPositionIndex;

    public ChangPlotLegendPositionCommand(PlotControl plot, int newIndex)
        : base(plot, "Change Plot Legend Position Command")
    {
        _newPositionIndex = newIndex;
    }

    public override bool Do()
    {
        _oldPositionIndex = _plot.PlotLegendPositionIndex;
        _plot.PlotLegendPositionIndex = _newPositionIndex;
        if (_newPositionIndex == _oldPositionIndex)
        {
            return false;
        }
        return true;
    }

    public override void Undo()
    {
        _plot.PlotLegendPositionIndex = _oldPositionIndex;
    }

    public override void Redo()
    {
        Do();
    }
}
