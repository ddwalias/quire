#nullable disable
using System.Reflection.Emit.Quire.Mpl;
using Quire.Mirror;

namespace Quire.Mpl.Run;

internal partial class CodeArg
{
    [MirrorVerified("MplRunCodeArgMechanicalDifferentialTests")]
    private Code m_beg;

    [MirrorVerified("MplRunCodeArgMechanicalDifferentialTests")]
    public override void BegArg(ILGenerator il, int i) => m_beg.EmitCode(il);

    [MirrorVerified("MplRunCodeArgMechanicalDifferentialTests")]
    private Code m_end;

    [MirrorVerified("MplRunCodeArgMechanicalDifferentialTests")]
    public override void EndArg(ILGenerator il, int i) => m_end.EmitCode(il);
}
