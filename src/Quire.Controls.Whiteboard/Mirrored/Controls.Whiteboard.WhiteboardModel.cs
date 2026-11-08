#nullable disable
#pragma warning disable

using System;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Xml;

namespace Quire.Controls.Whiteboard;

public partial class WhiteboardModel : IWhiteboardModel, ICloneable, IDisposable
{
    protected static IWhiteboardItemManager WhiteboardItemMgr => WhiteboardViewModel.WhiteboardItemMgr;

    public WhiteboardItemsCollection WhiteboardItems { get; private set; }

    public IWhiteboardItemPositionComparer PositionComparer { get; private set; }

    public WhiteboardModel(IWhiteboardItemPositionComparer itemPositionComparer = null)
    {
        PositionComparer = itemPositionComparer ?? new WhiteboardItemPositionComparer();
        WhiteboardItems = new WhiteboardItemsCollection();
    }

    public virtual object Clone()
    {
        WhiteboardModel whiteboardModel = new WhiteboardModel();
        CopyStateTo(whiteboardModel);
        return whiteboardModel;
    }

    protected void CopyStateTo(WhiteboardModel copy)
    {
        if (copy.WhiteboardItems.Count > 0)
        {
            copy.WhiteboardItems.RemoveAll();
        }
        foreach (IWhiteboardItem whiteboardItem in WhiteboardItems)
        {
            ICloneable cloneable = whiteboardItem as ICloneable;
            Point itemLocation = GetItemLocation(whiteboardItem);
            if (ShouldCloneItem(whiteboardItem))
            {
                IWhiteboardItem item;
                if (cloneable != null)
                {
                    item = (IWhiteboardItem)cloneable.Clone();
                }
                else
                {
                    item = (IWhiteboardItem)XmlHelper.CreateXamlCopy(whiteboardItem);
                }
                copy.AddItemAtLocation(item, itemLocation);
            }
        }
    }

    protected virtual bool ShouldCloneItem(IWhiteboardItem item)
    {
        return true;
    }

    public void InsertItemAtPosition(IWhiteboardItem item, int position)
    {
        WhiteboardItems.Insert(position, item);
    }

    public virtual void RemoveItem(IWhiteboardItem item)
    {
        WhiteboardItems.Remove(item);
    }

    protected virtual Point GetItemLocation(IWhiteboardItem item)
    {
        return WhiteboardItemMgr.GetItemLocation(item);
    }

    public virtual void AddItemAtLocation(IWhiteboardItem item, Point location)
    {
        WhiteboardItemMgr.SetItemLocation(item, location);
        InsertItemAtAppropriatePosition(item);
    }

    protected void InsertItemAtAppropriatePosition(IWhiteboardItem item)
    {
        if (WhiteboardItems.Count == 0)
        {
            InsertItemAtPosition(item, 0);
            return;
        }
        int num = WhiteboardItems.ToList().BinarySearch(item, PositionComparer);
        if (num > 0)
        {
            throw new InvalidProgramException("The item is already in the list");
        }
        InsertItemAtPosition(item, ~num);
    }

    public virtual void SetItemLocation(IWhiteboardItem item, Point location)
    {
        if (WhiteboardItemMgr.GetItemLocation(item) == location)
        {
            return;
        }
        WhiteboardItemMgr.SetItemLocation(item, location);
        UpdatePosition(item);
    }

    protected void UpdatePosition(IWhiteboardItem item)
    {
        int num = WhiteboardItems.IndexOf(item);
        IWhiteboardItem whiteboardItem = WhiteboardItems.FirstOrDefault((IWhiteboardItem wbItem) => wbItem != item && PositionComparer.Compare(wbItem, item) > 0);
        int num2 = WhiteboardItems.Count - 1;
        if (whiteboardItem != null)
        {
            num2 = WhiteboardItems.IndexOf(whiteboardItem) - ((num - WhiteboardItems.IndexOf(whiteboardItem) <= 0) ? 1 : 0);
        }
        if (num != num2)
        {
            WhiteboardItems.Move(num, num2);
        }
    }

    public bool HasOpenDescendantOleObjects => WhiteboardItems.GetRegionsOfType((IOleBaseObjectControl ole) => ole.IsOleOpen).IsNotEmpty();

    protected bool IsDisposed { get; private set; }

    public virtual void Dispose()
    {
        IsDisposed = true;
        WhiteboardItems.RemoveAll();
    }
}
