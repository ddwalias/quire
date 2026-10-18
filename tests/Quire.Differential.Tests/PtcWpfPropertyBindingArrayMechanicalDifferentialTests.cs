namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.PropertyBindingArray</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.PropertyBindingArray.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfPropertyBindingArrayMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000567() => Mechanical.Compare(typeof(global::Quire.Wpf.PropertyBindingArray), 0x06000567);
}
