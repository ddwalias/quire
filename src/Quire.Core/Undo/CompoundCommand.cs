#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Undo;

public partial class CompoundCommand
{
    [MirrorVerified("PtcUndoCompoundCommandMechanicalDifferentialTests")]
    private string _name;

    [MirrorVerified("PtcUndoCompoundCommandMechanicalDifferentialTests")]
    public CompoundCommand(string name, IEnumerable<IUndoableCommand> subCommands) : base(subCommands)
    {
        _name = name;
    }

    [MirrorVerified("PtcUndoCompoundCommandMechanicalDifferentialTests")]
    public CompoundCommand()
    {
        _name = string.Empty;
    }

    [MirrorVerified("PtcUndoCompoundCommandMechanicalDifferentialTests")]
    public string Name { get => _name; set => _name = value; }

    [MirrorVerified("Release IL")]
    public override string ToString() => string.Format("CompoundCommand Name: {0}", Name);
}
