namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.DiscreteRect</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.DiscreteRect.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcDiscreteRectMechanicalDifferentialTests
{
    [SkippableFact]
    public void TopLeft_get_0600018e() => Mechanical.Compare(typeof(global::Quire.DiscreteRect), 0x0600018e);

    [SkippableFact]
    public void TopLeft_set_0600018f() => Mechanical.Compare(typeof(global::Quire.DiscreteRect), 0x0600018f);

    [SkippableFact]
    public void BottomRight_get_06000190() => Mechanical.Compare(typeof(global::Quire.DiscreteRect), 0x06000190);

    [SkippableFact]
    public void BottomRight_set_06000191() => Mechanical.Compare(typeof(global::Quire.DiscreteRect), 0x06000191);
}
