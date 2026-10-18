namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Controls.Bookmarks.BookmarksManager</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Controls.Bookmarks.BookmarksManager.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcControlsBookmarksBookmarksManagerMechanicalDifferentialTests
{
    [SkippableFact]
    public void ctor_060002da() => Mechanical.Compare(typeof(global::Quire.Controls.Bookmarks.BookmarksManager), 0x060002da);

    [SkippableFact]
    public void DataChanged_add_060002d1() => Mechanical.Compare(typeof(global::Quire.Controls.Bookmarks.BookmarksManager), 0x060002d1);

    [SkippableFact]
    public void DataChanged_remove_060002d2() => Mechanical.Compare(typeof(global::Quire.Controls.Bookmarks.BookmarksManager), 0x060002d2);
}
