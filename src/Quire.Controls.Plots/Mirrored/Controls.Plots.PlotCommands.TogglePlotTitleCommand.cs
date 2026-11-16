using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal sealed partial class TogglePlotTitleCommand : IUndoableCommand
{
    private readonly PlotControl _plot;

    private bool _oldValue;

    private bool _newValue;

    public string Name => "Toggle Plot Title";

    public TogglePlotTitleCommand(PlotControl plot)
    {
        _plot = plot;
    }

    public bool Do()
    {
        if (_plot?._plotTitle == null || _plot.Model?.PlotTitleModel == null)
        {
            return false;
        }
        _oldValue = _plot.Model.PlotTitleModel.ShowTitle;
        _newValue = !_oldValue;
        if (_newValue == _oldValue)
        {
            return false;
        }
        _plot.Model.PlotTitleModel.ShowTitle = _newValue;
        return true;
    }

    public void Undo()
    {
        if (_plot?.Model?.PlotTitleModel == null)
        {
            return;
        }
        _plot.Model.PlotTitleModel.ShowTitle = _oldValue;
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
