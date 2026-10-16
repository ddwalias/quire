#nullable disable
using System.Reflection.Emit.Quire.Mpl;
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class CodeFunction
{
    [MirrorVerified("MplRunCodeFunctionMechanicalDifferentialTests")]
    private Code m_call;

    [MirrorVerified("MplRunCodeFunctionMechanicalDifferentialTests")]
    public override void EmitCode(ILGenerator il) => m_call.EmitCode(il);
}
