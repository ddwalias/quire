#nullable disable
using System.Reflection.Emit.Quire.Mpl;
using Quire.Mirror;

namespace Quire.Mpl.Run;

internal partial class InstructionSeq
{
    [MirrorVerified("InstructionEmitCodeDifferentialTests")]
    public override void EmitCode(ILGenerator il)
    {
        for (int i = 0; i < m_ops.Length; i++)
        {
            m_ops[i].EmitCode(il);
        }
    }

    [MirrorVerified("MplRunInstructionSeqMechanicalDifferentialTests")]
    private Instruction[] m_ops;

    [MirrorVerified("MplRunInstructionSeqMechanicalDifferentialTests")]
    private InstructionSeq(Instruction[] ops)
    {
        m_ops = ops;
    }
}
