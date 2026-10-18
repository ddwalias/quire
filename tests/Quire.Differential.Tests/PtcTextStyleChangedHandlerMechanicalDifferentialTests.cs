namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.TextStyleChangedHandler</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.TextStyleChangedHandler.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcTextStyleChangedHandlerMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_06000360() => Mechanical.Compare(typeof(global::Quire.TextStyleChangedHandler), 0x06000360);

    [SkippableFact]
    public void StyleChanged_add_0600035a() => Mechanical.Compare(typeof(global::Quire.TextStyleChangedHandler), 0x0600035a);

    [SkippableFact]
    public void StyleChanged_remove_0600035b() => Mechanical.Compare(typeof(global::Quire.TextStyleChangedHandler), 0x0600035b);

    [SkippableFact]
    public void StyleCreateRequest_add_0600035c() => Mechanical.Compare(typeof(global::Quire.TextStyleChangedHandler), 0x0600035c);

    [SkippableFact]
    public void StyleCreateRequest_remove_0600035d() => Mechanical.Compare(typeof(global::Quire.TextStyleChangedHandler), 0x0600035d);
}
