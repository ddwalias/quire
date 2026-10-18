namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Wpf.ResourceString</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Wpf.ResourceString.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfResourceStringMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_0600057c() => Mechanical.Compare(typeof(global::Quire.Wpf.ResourceString), 0x0600057c);

    [SkippableFact]
    public void Value_get_06000576() => Mechanical.Compare(typeof(global::Quire.Wpf.ResourceString), 0x06000576);

    [SkippableFact]
    public void Value_set_06000577() => Mechanical.Compare(typeof(global::Quire.Wpf.ResourceString), 0x06000577);

    [SkippableFact]
    public void StringFormat_get_06000578() => Mechanical.Compare(typeof(global::Quire.Wpf.ResourceString), 0x06000578);

    [SkippableFact]
    public void StringFormat_set_06000579() => Mechanical.Compare(typeof(global::Quire.Wpf.ResourceString), 0x06000579);

    [SkippableFact]
    public void String_get_0600057a() => Mechanical.Compare(typeof(global::Quire.Wpf.ResourceString), 0x0600057a);
}
