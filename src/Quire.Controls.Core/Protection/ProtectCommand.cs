#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.Protection;

public partial class ProtectCommand
{
    [MirrorVerified("PtcControlsCoreProtectionProtectCommandMechanicalDifferentialTests")]
    public void Redo() => Do();

    [MirrorVerified("PtcControlsCoreProtectionProtectCommandMechanicalDifferentialTests")]
    public string Name => "Set Control Protection";
}
