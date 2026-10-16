namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Mpl.Run.InstructionSeq</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Mpl.Run.InstructionSeq.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MplRunInstructionSeqMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000350() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.InstructionSeq), 0x06000350);
}
