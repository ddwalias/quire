namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.PropertyBinding</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.PropertyBinding.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfPropertyBindingMechanicalDifferentialTests
{
    [SkippableFact]
    public void Set_06000564() => Mechanical.Compare(typeof(global::Quire.Wpf.PropertyBinding), 0x06000564);

    [SkippableFact]
    public void Clear_06000565() => Mechanical.Compare(typeof(global::Quire.Wpf.PropertyBinding), 0x06000565);

    [SkippableFact]
    public void ctor_06000566() => Mechanical.Compare(typeof(global::Quire.Wpf.PropertyBinding), 0x06000566);
}
