#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.BookmarkTags;

public static partial class BookmarkTag
{
    [MirrorVerified("PtcControlsCoreBookmarkTagsBookmarkTagMechanicalDifferentialTests")]
    public static readonly DependencyProperty BookmarkTagProperty;

    [MirrorVerified("PtcControlsCoreBookmarkTagsBookmarkTagMechanicalDifferentialTests")]
    public static void SetBookmarkTag(DependencyObject element, string tag) => element.SetValue(BookmarkTagProperty, tag);

    [MirrorVerified("PtcControlsCoreBookmarkTagsBookmarkTagMechanicalDifferentialTests")]
    public static void ClearBookmarkTag(DependencyObject element) => element.ClearValue(BookmarkTagProperty);
}
