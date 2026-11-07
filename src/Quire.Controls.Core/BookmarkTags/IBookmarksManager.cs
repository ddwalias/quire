#nullable disable
using System;

namespace Quire.Controls.Core.BookmarkTags;

public partial interface IBookmarksManager
{
    event Action DataChanged;
    void SetBookmarkTagCommand(IWhiteboardItem item, string oldTagValue, string tagValue);
    void RemoveBookmarkTagCommand(IWhiteboardItem item, string oldTagValue);
}
