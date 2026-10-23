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

    public void Dispose()
    {
        // Keep monitor locals before the command array, as required by the original Release IL slots.
        Stack<CommandScope> scopeStack = _scopeStack;
        bool lockTaken = false;
        try
        {
            global::System.Threading.Monitor.Enter(scopeStack, ref lockTaken);
            DebugTools.DevAssert(CommandBuffer.CurrentInstance == _buffer, null);
            DebugTools.DevAssert(_scopeStack.Peek() == this, null);
            _scopeStack.Pop();
        }
        finally
        {
            if (lockTaken)
            {
                global::System.Threading.Monitor.Exit(scopeStack);
            }
        }
        CommandBuffer.CurrentInstance = _parentBuffer;
        if (_parentBuffer != null)
        {
            IUndoableCommand[] undoCommands = _buffer.GetUndoCommands();
            if (undoCommands.Length != 0)
            {
                if (_targetCommand != null)
                {
                    _targetCommand.Clear();
                    _targetCommand.Name = _name;
                    _targetCommand.AddRange(undoCommands);
                }
                else if (undoCommands.Length == 1)
                {
                    _parentBuffer.PushCommand(undoCommands[0]);
                }
                else
                {
                    _parentBuffer.PushCommand(new CompoundCommand(_name, undoCommands));
                }
            }
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
