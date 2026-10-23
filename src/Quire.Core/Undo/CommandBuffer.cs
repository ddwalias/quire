#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Undo;

public partial class CommandBuffer
{
    [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
    private static CommandBuffer _currentInstance;

    [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
    public static CommandBuffer CurrentInstance { get => _currentInstance; set => _currentInstance = value; }

    [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
    private readonly Stack<IUndoableCommand> _undoStack = new Stack<IUndoableCommand>();

    public CommandBuffer() { }

    [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
    public static int UndoStackDepth => _currentInstance._undoStack.Count;

    [MirrorVerified("Release IL")]
    public static bool CanUndo
    {
        get
        {
            if (_currentInstance != null)
            {
                return _currentInstance._undoStack.Count > 0;
            }
            return false;
        }
    }

    [MirrorVerified("Release IL")]
    public static bool CanRedo
    {
        get
        {
            if (_currentInstance != null)
            {
                return _currentInstance._redoStack.Count > 0;
            }
            return false;
        }
    }

    [MirrorVerified("Release IL")]
    private static IUndoableCommand NextUndoCommand
    {
        get
        {
            if (_currentInstance._undoStack.Count <= 0)
            {
                return null;
            }
            return _currentInstance._undoStack.Peek();
        }
    }

    [MirrorVerified("Release IL")]
    private static IUndoableCommand NextRedoCommand
    {
        get
        {
            if (_currentInstance._redoStack.Count <= 0)
            {
                return null;
            }
            return _currentInstance._redoStack.Peek();
        }
    }

    [MirrorVerified("Release IL")]
    private static void FireCommandEvent(EventHandler<CommandEventArgs> ev, CommandEventArgs args)
    {
        if (ev != null)
        {
            ev(_currentInstance, args);
        }
    }

    [MirrorVerified("Release IL")]
    public static void Undo()
    {
        if (!CanUndo)
        {
            return;
        }
        IUndoableCommand command = _currentInstance._undoStack.Pop();
        CommandEventArgs args = new CommandEventArgs(command, (CommandEventArgs.ActionTypeEnum)1);
        FireCommandEvent(_currentInstance.BeforeCommand, args);
        CompoundCommand compoundCommand = new CompoundCommand();
        try
        {
            using (new CommandScope(compoundCommand))
            {
                command.Undo();
            }
        }
        finally
        {
            _currentInstance._redoStack.Push(command);
            FireCommandEvent(_currentInstance.AfterCommand, args);
        }
    }

    [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
    public event EventHandler<CommandEventArgs> BeforeCommand;

    [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
    public event EventHandler<CommandEventArgs> AfterCommand;

    partial class MasterCommand
    {
        [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
        public override bool Do() => throw new InvalidOperationException("Do method should never be called on a MasterCommand.");

        [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
        private readonly IUndoableCommand _top;

        [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
        public IUndoableCommand Top => _top;
    }

    [MirrorVerified("Release IL")]
    private readonly Stack<IUndoableCommand> _redoStack = new Stack<IUndoableCommand>();

    [MirrorVerified("Release IL")]
    public static void Clear()
    {
        _currentInstance._redoStack.Clear();
        _currentInstance._undoStack.Clear();
    }

    [MirrorVerified("Release IL")]
    public static void Reset()
    {
        _instances.Clear();
        CurrentInstance = _defaultBuffer = new CommandBuffer();
    }

    public static CommandBuffer SwitchBuffer(object key)
    {
        if (_currentInstance != _defaultBuffer && !_instances.ContainsValue(_currentInstance))
        {
            global::Quire.DebugTools.FailAssertion("Old instance is missing");
        }
        else if (!_instances.ContainsKey(key))
        {
            global::Quire.DebugTools.FailAssertion("New instance is missing");
        }
        else
        {
            CurrentInstance = _instances[key];
        }
        return CurrentInstance;
    }

    [MirrorVerified("Release IL")]
    public static bool Execute(string name, DelegateFunction0<bool> doMethod, Delegate0 undoMethod) => Execute(new UndoableCommand(name, doMethod, undoMethod), null);

    [MirrorVerified("Release IL")]
    public static bool Execute(string name, DelegateFunction0<bool> doMethod, Delegate0 undoMethod, Delegate0 redoMethod) => Execute(new UndoableCommand(name, doMethod, undoMethod, redoMethod), null);

    [MirrorVerified("Release IL")]
    public void PushCommand(IUndoableCommand command)
    {
        _currentInstance._redoStack.Clear();
        _currentInstance._undoStack.Push(command);
    }

    public IUndoableCommand[] GetUndoCommands()
    {
        IUndoableCommand[] commands = _undoStack.ToArray();
        Array.Reverse(commands);
        return commands;
    }

    public IUndoableCommand[] GetRedoCommands()
    {
        IUndoableCommand[] commands = _redoStack.ToArray();
        Array.Reverse(commands);
        return commands;
    }

    partial class MasterCommand
    {
        [MirrorVerified("Release IL")]
        public MasterCommand(IUndoableCommand top)
        {
            Name = top.Name;
            _top = top;
        }

        [MirrorVerified("Release IL")]
        public override void Undo()
        {
            base.UndoSideEffects();
            _top.Undo();
        }

        [MirrorVerified("Release IL")]
        public override void Redo()
        {
            _top.Redo();
            base.RedoSideEffects();
        }
    }
}
