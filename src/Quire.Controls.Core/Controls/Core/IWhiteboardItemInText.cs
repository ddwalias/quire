#nullable disable
using System.Windows.Input;
using Quire.Controls.Core.BookmarkTags;

namespace Quire.Controls.Core;

public partial interface IWhiteboardItemInText
{
    bool IsEmbedded { set; }
    IBookmarksManager BookmarksManager { get; set; }
    bool CanNavigate(Direction direction);
    bool CanReturn();
    bool CanTraverse(FocusNavigationDirection direction);
}
