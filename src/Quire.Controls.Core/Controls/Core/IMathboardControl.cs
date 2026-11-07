#nullable disable
using System.Windows;
using Quire.Controls.Core.BookmarkTags;

namespace Quire.Controls.Core;

public partial interface IMathboardControl
{
    IBookmarksManager BookmarksManager { get; set; }
    string GetBlockEquationTypeAttributeName(IWhiteboardItem item);
    bool HasItemInViewPort(Rect viewPortRect);
}
