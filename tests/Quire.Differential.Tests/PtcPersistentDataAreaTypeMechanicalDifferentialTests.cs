namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.PersistentData.AreaType</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.PersistentData.AreaType.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcPersistentDataAreaTypeMechanicalDifferentialTests
{
    [SkippableFact]
    public void GetXmlSerializer_0600087f()
    {
        Skip.If(true, "The mirrored serializer graph is incomplete: worksheetRegionType includes XmlNamespaceManager and other mapped types without Mathcad XML annotations.");
        Mechanical.Compare(typeof(global::Quire.PersistentData.AreaType), 0x0600087f);
    }

    [SkippableFact]
    public void regions_get_06000873() => Mechanical.Compare(typeof(global::Quire.PersistentData.AreaType), 0x06000873);

    [SkippableFact]
    public void regions_set_06000874() => Mechanical.Compare(typeof(global::Quire.PersistentData.AreaType), 0x06000874);

    [SkippableFact]
    public void ProtectionData_get_06000875() => Mechanical.Compare(typeof(global::Quire.PersistentData.AreaType), 0x06000875);

    [SkippableFact]
    public void ProtectionData_set_06000876() => Mechanical.Compare(typeof(global::Quire.PersistentData.AreaType), 0x06000876);

    [SkippableFact]
    public void IsCollapsed_get_06000877() => Mechanical.Compare(typeof(global::Quire.PersistentData.AreaType), 0x06000877);

    [SkippableFact]
    public void IsCollapsed_set_06000878() => Mechanical.Compare(typeof(global::Quire.PersistentData.AreaType), 0x06000878);

    [SkippableFact]
    public void IsBorderVisible_get_06000879() => Mechanical.Compare(typeof(global::Quire.PersistentData.AreaType), 0x06000879);

    [SkippableFact]
    public void IsBorderVisible_set_0600087a() => Mechanical.Compare(typeof(global::Quire.PersistentData.AreaType), 0x0600087a);

    [SkippableFact]
    public void AreaHeight_get_0600087b() => Mechanical.Compare(typeof(global::Quire.PersistentData.AreaType), 0x0600087b);

    [SkippableFact]
    public void AreaHeight_set_0600087c() => Mechanical.Compare(typeof(global::Quire.PersistentData.AreaType), 0x0600087c);
}
