#nullable disable
using System;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.BookmarkTags;
using Quire.Mirror;
using Quire.Undo;

namespace Quire.Controls.Bookmarks;

public partial class BookmarksManager
{
    [MirrorVerified("PtcControlsBookmarksBookmarksManagerMechanicalDifferentialTests")]
    public event Action DataChanged;

    private void FireDataChanged()
    {
        DataChanged?.Invoke();
    }

    private void SetRegionBookmarkTag(IWhiteboardItem item, string tagValue)
    {
        if (tagValue.IsNotEmpty())
        {
            BookmarkTag.SetBookmarkTag(item as DependencyObject, tagValue);
        }
        else
        {
            BookmarkTag.ClearBookmarkTag(item as DependencyObject);
        }
        FireDataChanged();
    }

    private UndoableCommand CreateBookmarkTagCommand(IWhiteboardItem item, string oldTagValue, string tagValue)
    {
        return new UndoableCommand("Set Bookmark Tag", DoAct, UndoAct, RedoAct);

        bool DoAct()
        {
            SetRegionBookmarkTag(item, tagValue);
            return true;
        }

        void UndoAct()
        {
            SetRegionBookmarkTag(item, oldTagValue);
        }

        void RedoAct()
        {
            SetRegionBookmarkTag(item, tagValue);
        }
    }

    public void SetBookmarkTagCommand(IWhiteboardItem item, string oldTagValue, string tagValue)
    {
        CommandBuffer.Execute(CreateBookmarkTagCommand(item, oldTagValue, tagValue), null);
    }

    private void ClearRegionBookmarkTag(IWhiteboardItem item)
    {
        BookmarkTag.ClearBookmarkTag(item as DependencyObject);
        FireDataChanged();
    }

    private UndoableCommand CreateRemoveBookmarkTagCommand(IWhiteboardItem item, string oldTagValue)
    {
        return new UndoableCommand("Clear Bookmark Tag", DoAct, UndoAct, RedoAct);

        bool DoAct()
        {
            ClearRegionBookmarkTag(item);
            return true;
        }

        void UndoAct()
        {
            SetRegionBookmarkTag(item, oldTagValue);
        }

        void RedoAct()
        {
            ClearRegionBookmarkTag(item);
        }
    }

    public void RemoveBookmarkTagCommand(IWhiteboardItem item, string oldTagValue)
    {
        CommandBuffer.Execute(CreateRemoveBookmarkTagCommand(item, oldTagValue), null);
    }

    [MirrorVerified("PtcControlsBookmarksBookmarksManagerMechanicalDifferentialTests")]
    public BookmarksManager() { }
}
