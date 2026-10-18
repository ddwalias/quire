namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.SingleRichTextCopyVisitor</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.SingleRichTextCopyVisitor.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcSingleRichTextCopyVisitorMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_0600000c() => Mechanical.Compare(typeof(global::Quire.SingleRichTextCopyVisitor), 0x0600000c);

    [SkippableFact]
    public void Visit_0600000d() => Mechanical.Compare(typeof(global::Quire.SingleRichTextCopyVisitor), 0x0600000d);
}
