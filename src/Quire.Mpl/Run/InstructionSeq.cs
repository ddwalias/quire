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
}
