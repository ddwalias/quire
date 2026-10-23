#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Undo;

public partial class CommandScope
{
    public CommandScope(string name, bool canUndoTreeOperations)
    {
        _canUndoTreeOperations = canUndoTreeOperations;
        _parentBuffer = CommandBuffer.CurrentInstance;
        CommandBuffer.CurrentInstance = (_buffer = new CommandBuffer());
        _name = name;
        lock (_scopeStack)
        {
            _scopeStack.Push(this);
        }
    }

    [MirrorVerified("PtcUndoCommandScopeMechanicalDifferentialTests")]
    public CommandScope(string name) : this(name, false) { }

    [MirrorVerified("PtcUndoCommandScopeMechanicalDifferentialTests")]
    public CommandScope(CompoundCommand targetCommand) : this(targetCommand, false) { }

    [MirrorVerified("Release IL")]
    private readonly CompoundCommand _targetCommand;

    [MirrorVerified("Release IL")]
    public CommandScope(CompoundCommand targetCommand, bool canUndoTreeOperations) : this(targetCommand.Name, canUndoTreeOperations)
    {
        _targetCommand = targetCommand;
    }
}
