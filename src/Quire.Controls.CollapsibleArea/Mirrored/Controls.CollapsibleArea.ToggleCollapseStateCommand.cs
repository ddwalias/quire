#nullable disable
#pragma warning disable

namespace Quire.Controls.CollapsibleArea;

public partial class ToggleCollapseStateCommand
{
    private readonly ICollapsibleAreaModel _model;

    private readonly bool _isCollapsedCurrent;

    public string Name => "Toggle Collapse State";

    public ToggleCollapseStateCommand(ICollapsibleAreaModel model)
    {
        _model = model;
        _isCollapsedCurrent = _model.IsCollapsed;
    }

    public bool Do()
    {
        return _model.ToggleCollapseState();
    }

    public void Undo()
    {
        DebugTools.DevAssert(_model.CanToggleCollapseState && !_isCollapsedCurrent == _model.IsCollapsed, null);
        _model.ToggleCollapseState();
    }

    public void Redo()
    {
        Do();
    }
}
