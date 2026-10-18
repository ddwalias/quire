#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Bookmarks;

public partial class BookmarksManager
{
    [MirrorVerified("PtcControlsBookmarksBookmarksManagerMechanicalDifferentialTests")]
    public BookmarksManager() { }

    [MirrorVerified("PtcControlsBookmarksBookmarksManagerMechanicalDifferentialTests")]
    public event Action DataChanged;
}
