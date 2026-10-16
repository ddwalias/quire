namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Mpl.Run.ModuleInterfaceAttribute</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Mpl.Run.ModuleInterfaceAttribute.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class MplRunModuleInterfaceAttributeMechanicalDifferentialTests
{
    [SkippableFact]
    public void ToString_0600000a() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.ModuleInterfaceAttribute), 0x0600000a);

    [SkippableFact]
    public void ctor_0600000b() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.ModuleInterfaceAttribute), 0x0600000b);

    [SkippableFact]
    public void Interface_get_06000009() => Mechanical.Compare(typeof(global::Quire.Mpl.Run.ModuleInterfaceAttribute), 0x06000009);
}
