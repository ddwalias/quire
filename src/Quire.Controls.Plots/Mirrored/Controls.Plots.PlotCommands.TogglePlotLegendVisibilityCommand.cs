using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal sealed partial class TogglePlotLegendVisibilityCommand : IUndoableCommand
{
    private readonly PlotControl _plot;

    private bool _oldValue;

    private bool _newValue;

    public string Name => "Toggle Plot Legend Visibility";

    public TogglePlotLegendVisibilityCommand(PlotControl plot)
    {
        _plot = plot;
    }

    public bool Do()
    {
        if (_plot?.Model?.PlotLegendModel == null)
        {
            return false;
        }
        _oldValue = _plot.Model.PlotLegendModel.ShowPlotLegend;
        _newValue = !_oldValue;
        if (_newValue == _oldValue)
        {
            return false;
        }
        _plot.Model.PlotLegendModel.ShowPlotLegend = _newValue;
        return true;
    }

    public void Undo()
    {
        if (_plot?.Model?.PlotLegendModel == null)
        {
            return;
        }
        _plot.Model.PlotLegendModel.ShowPlotLegend = _oldValue;
    }

    public void Redo()
    {
        Do();
    }

    public void Execute()
    {
        CommandBuffer.Execute(this);
    }
}
