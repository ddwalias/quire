namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.MouseEventGenerator</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.MouseEventGenerator.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcMouseEventGeneratorMechanicalDifferentialTests
{
    [SkippableFact]
    public void GenerateMouseUp_06000224() => Mechanical.Compare(typeof(global::Quire.MouseEventGenerator), 0x06000224);

    [SkippableFact]
    public void GenerateMouseDown_06000225() => Mechanical.Compare(typeof(global::Quire.MouseEventGenerator), 0x06000225);

    [SkippableFact]
    public void GenerateMouseClick_06000228() => Mechanical.Compare(typeof(global::Quire.MouseEventGenerator), 0x06000228);

    [SkippableFact]
    public void ctor_0600022d() => Mechanical.Compare(typeof(global::Quire.MouseEventGenerator), 0x0600022d);
}
