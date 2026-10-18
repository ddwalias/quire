namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Core.TextStylesCollectionManager</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Core.TextStylesCollectionManager.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsCoreTextStylesCollectionManagerMechanicalDifferentialTests
{
    [SkippableFact]
    public void TextStyleChangedHandler_get_06000714() => Mechanical.Compare(typeof(global::Quire.Controls.Core.TextStylesCollectionManager), 0x06000714);

    [SkippableFact]
    public void StyleCollectionChanged_add_0600070e() => Mechanical.Compare(typeof(global::Quire.Controls.Core.TextStylesCollectionManager), 0x0600070e);

    [SkippableFact]
    public void StyleCollectionChanged_remove_0600070f() => Mechanical.Compare(typeof(global::Quire.Controls.Core.TextStylesCollectionManager), 0x0600070f);

    [SkippableFact]
    public void PropertyChanged_add_06000710() => Mechanical.Compare(typeof(global::Quire.Controls.Core.TextStylesCollectionManager), 0x06000710);

    [SkippableFact]
    public void PropertyChanged_remove_06000711() => Mechanical.Compare(typeof(global::Quire.Controls.Core.TextStylesCollectionManager), 0x06000711);
}
