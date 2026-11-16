using Quire.Controls.Plots.ViewModel;
using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class ToggleAxisTitleOrientationCommand(PlotTitle axisTitle) : IUndoableCommand
{
    private bool _oldValue;

    private bool _newValue;

    public string Name => "Toggle Axis Title Orientation";

    public bool Do()
    {
        PlotTitleViewModel plotTitleViewModel = axisTitle?.ViewModel;
        if (plotTitleViewModel == null)
        {
            return false;
        }
        _oldValue = plotTitleViewModel.IsVertical;
        _newValue = !_oldValue;
        if (_newValue == _oldValue)
        {
            return false;
        }
        plotTitleViewModel.IsVertical = _newValue;
        return true;
    }

    public void Undo()
    {
        PlotTitleViewModel plotTitleViewModel = axisTitle?.ViewModel;
        if (plotTitleViewModel == null)
        {
            return;
        }
        plotTitleViewModel.IsVertical = _oldValue;
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
