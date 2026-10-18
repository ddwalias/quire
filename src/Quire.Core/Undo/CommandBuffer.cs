#nullable disable
using Quire.Mirror;

namespace Quire.Undo;

public partial class CommandBuffer
{
    [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
    private static CommandBuffer _currentInstance;

    [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
    public static CommandBuffer CurrentInstance { get => _currentInstance; set => _currentInstance = value; }

    [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
    private readonly Stack<IUndoableCommand> _undoStack;

    [MirrorVerified("PtcUndoCommandBufferMechanicalDifferentialTests")]
    public static int UndoStackDepth => _currentInstance._undoStack.Count;

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
}
