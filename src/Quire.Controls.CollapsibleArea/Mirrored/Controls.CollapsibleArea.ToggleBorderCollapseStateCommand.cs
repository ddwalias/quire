#nullable disable
#pragma warning disable

namespace Quire.Controls.CollapsibleArea;

public partial class ToggleBorderCollapseStateCommand
{
    private readonly ICollapsibleAreaModel _model;

    public string Name => "Border Visibility Command";

    public ToggleBorderCollapseStateCommand(ICollapsibleAreaModel model)
    {
        _model = model;
    }

    public bool Do()
    {
        return _model.ToggleBorderVisibleState();
    }

    public void Undo()
    {
        _model.ToggleBorderVisibleState();
    }

    public void Redo()
    {
        Do();
    }
}
