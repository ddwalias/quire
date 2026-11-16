using Quire.Undo;

namespace Quire.Controls.Plots.PlotCommands;

internal partial class ToggleAxisTitleVisibilityCommand : IUndoableCommand
{
    private readonly PlotTitle _axisTitle;

    private readonly bool _oldValue;

    private readonly bool _newValue;

    public string Name => "Toggle Axis Title Visibility";

    public ToggleAxisTitleVisibilityCommand(PlotTitle axisTitle)
    {
        _axisTitle = axisTitle;
        _oldValue = axisTitle.ViewModel.ShowTitle;
        _newValue = !_oldValue;
    }

    public bool Do()
    {
        if (_oldValue == _newValue)
        {
            return false;
        }
        if (_axisTitle.ViewModel != null)
        {
            _axisTitle.ViewModel.ShowTitle = _newValue;
        }
        return true;
    }

    public void Undo()
    {
        if (_axisTitle.ViewModel != null)
        {
            _axisTitle.ViewModel.ShowTitle = _oldValue;
        }
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
