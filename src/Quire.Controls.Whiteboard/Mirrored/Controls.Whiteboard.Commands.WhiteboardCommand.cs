#nullable disable
#pragma warning disable

using Quire.Undo;

namespace Quire.Controls.Whiteboard.Commands;

public abstract partial class WhiteboardCommand : IUndoableCommand
{
    protected IWhiteboardViewModel _viewModel;

    protected WhiteboardViewModel ViewModel => _viewModel as WhiteboardViewModel;

    public string Name { get; private set; }

    protected WhiteboardCommand(IWhiteboardViewModel viewModel, string commandName)
    {
        _viewModel = viewModel;
        Name = commandName;
    }

    public abstract bool Do();

    public abstract void Undo();

    public abstract void Redo();

    public bool Execute()
    {
        return CommandBuffer.Execute(this);
    }
}
