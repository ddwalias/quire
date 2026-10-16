#nullable disable
using System.Reflection.Emit.Quire.Mpl;
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class CodeFunction
{
    [MirrorVerified("CodeFunctionDifferentialTests")]
    private Code[] m_args;

    [MirrorVerified("MplRunCodeFunctionMechanicalDifferentialTests")]
    private Code m_call;

    [MirrorVerified("MplRunCodeFunctionMechanicalDifferentialTests")]
    public override void EmitCode(ILGenerator il) => m_call.EmitCode(il);

    [MirrorVerified("CodeFunctionDifferentialTests")]
    public override void BegArg(ILGenerator il, int i)
    {
        m_args[i].BegArg(il, i);
    }

    [MirrorVerified("CodeFunctionDifferentialTests")]
    public override void EndArg(ILGenerator il, int i)
    {
        m_args[i].EndArg(il, i);
    }

    [MirrorVerified("CodeFunctionDifferentialTests")]
    public CodeFunction(Code call, Code[] args)
    {
        m_call = call;
        m_args = args;
    }
}
