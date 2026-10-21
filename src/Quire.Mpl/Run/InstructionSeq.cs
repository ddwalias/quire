#nullable disable
using Quire.Mirror;

namespace Quire.Mpl.Run;

internal partial class InstructionSeq
{
    [MirrorVerified("MplRunInstructionSeqMechanicalDifferentialTests")]
    private Instruction[] m_ops;

    [MirrorVerified("MplRunInstructionSeqMechanicalDifferentialTests")]
    private InstructionSeq(Instruction[] ops)
    {
        m_ops = ops;
    }

    [MirrorVerified("MplRunInstructionSeqMechanicalDifferentialTests")]
    public override void EmitCode(System.Reflection.Emit.Quire.Mpl.ILGenerator g)
    {
        for (int i = 0; i < m_ops.Length; i++)
        {
            m_ops[i].EmitCode(g);
        }
    }
}
