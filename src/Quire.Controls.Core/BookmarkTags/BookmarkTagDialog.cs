#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core.BookmarkTags;

public partial class BookmarkTagDialog
{
    [MirrorVerified("PtcControlsCoreBookmarkTagsBookmarkTagDialogMechanicalDifferentialTests")]
    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, this, handler);

    [MirrorVerified("PtcControlsCoreBookmarkTagsBookmarkTagDialogMechanicalDifferentialTests")]
    private BookmarkTagViewModel ViewModel { get; }
}
