#nullable disable
using Quire.Mirror;

namespace Quire.Undo;

public partial class CommandScope
{
    [MirrorVerified("PtcUndoCommandScopeMechanicalDifferentialTests")]
    public CommandScope(string name) : this(name, false) { }

    [MirrorVerified("PtcUndoCommandScopeMechanicalDifferentialTests")]
    public CommandScope(CompoundCommand targetCommand) : this(targetCommand, false) { }
}
