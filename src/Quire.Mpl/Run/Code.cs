#nullable disable
using System.Reflection.Emit.Quire.Mpl;
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class Code
{
    [MirrorVerified("MplRunCodeMechanicalDifferentialTests")]
    public virtual void EmitCode(ILGenerator il) { }

    [MirrorVerified("MplRunCodeMechanicalDifferentialTests")]
    public virtual void BegArg(ILGenerator il, int i) { }

    [MirrorVerified("MplRunCodeMechanicalDifferentialTests")]
    public virtual void EndArg(ILGenerator il, int i) { }

    [MirrorVerified("MplRunCodeMechanicalDifferentialTests")]
    public Code() { }
}
