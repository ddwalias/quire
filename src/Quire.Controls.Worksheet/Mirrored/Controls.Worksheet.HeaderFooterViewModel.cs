using System;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.ExcelComponent;
using Quire.Controls.Whiteboard;

namespace Quire.Controls.Worksheet;

internal partial class HeaderFooterViewModel
{
    public WhiteboardItemsCollection CommonItems { get; private set; }

    public WhiteboardItemsCollection UniqueItems { get; private set; }

    public Thickness Margin
    {
        get
        {
            return _margin;
        }
        set
        {
            _margin = value;
            OnPropertyChanged("Margin");
        }
    }

    public override Point DefaultInsertionPoint
    {
        get
        {
            return new Point(0.0, 1.0);
        }
    }

    protected override Size StrictMinimumSize
    {
        get
        {
            return _strictMinimumSize;
        }
    }

    public void UpdateItemLocationsAfterGridSizeChanged(Size oldGridCellSize)
    {
        foreach (IWhiteboardItem item in WhiteboardItems.ToList())
        {
            Point itemLocation = GetItemLocation(item);
            Point value = new Point(itemLocation.X / oldGridCellSize.Width, itemLocation.Y / oldGridCellSize.Height);
            SetItemLocation(item, Grid.ConvertGridCoordinatesToDips(value));
        }
    }

    public bool WorksheetHandleDelete()
    {
        if (SelectedItems.Count() > 0)
        {
            DeleteSelectedItems();
            return true;
        }
        return false;
    }

    protected override void HandleItemRemoved(IWhiteboardItem item)
    {
        base.HandleItemRemoved(item);
        if (IsUniqueItem(item))
        {
            UniqueItems.Remove(item);
        }
        else
        {
            CommonItems.Remove(item);
        }
        if (item is IResizeToFitWhiteboardItem resizeToFitWhiteboardItem)
        {
            resizeToFitWhiteboardItem.RequestResizeToFitOrClearRestriction -= HandleRequestResizeToFitOrClearRestriction;
        }
    }

    private void HandleRequestResizeToFitOrClearRestriction(IResizeToFitWhiteboardItem element)
    {
        ResizeItemToFit(element, assertIsNotReady: true);
    }
}
