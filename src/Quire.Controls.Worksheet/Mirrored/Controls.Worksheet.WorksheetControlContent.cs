using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class WorksheetControlContent
{
    public Point GetItemGridPosition(IWhiteboardItem item)
    {
        throw new NotImplementedException();
    }

    public void SetItemGridPosition(IWhiteboardItem item, Point location)
    {
        throw new NotImplementedException();
    }

    public void AddItem(IWhiteboardItem item)
    {
        throw new NotImplementedException();
    }

    public void AddItems(Dictionary<IWhiteboardItem, Point> addItemsAtLocation)
    {
        AddItemsCommand command = new AddItemsCommand(this, addItemsAtLocation);
        command.Execute();
    }

    public void RemoveItem(IWhiteboardItem item)
    {
        _model.RemoveItem(item);
    }

    public void TabInto(FocusNavigationDirection dir)
    {
        throw new NotImplementedException();
    }

    public void CommitItem(IWhiteboardItem item)
    {
        _model.CommitItem(item);
    }

    public void SelectAllItems()
    {
        SelectItems(WhiteboardItems);
    }

    public void SelectItem(IWhiteboardItem item)
    {
        SelectItems(new IWhiteboardItem[1] { item });
    }

    public void UnSelectItem(IWhiteboardItem item)
    {
        UnSelectItems(new IWhiteboardItem[1] { item });
    }

    public void PasteItems(IDictionary<IWhiteboardItem, Point> pasteItemsAtLocation)
    {
        PasteItemsCommand command = new PasteItemsCommand(this, pasteItemsAtLocation);
        command.Execute();
    }

    public Vector? NavigateInto(Direction direction, Point point)
    {
        throw new NotImplementedException();
    }

    public void UpdateSizeLimits(Size maximumSize)
    {
        throw new NotImplementedException();
    }

    public void InsertVerticalSpace(object sender, ExecutedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void DeleteVerticalSpaceAbove(object sender, ExecutedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void DeleteVerticalSpaceBelow(object sender, ExecutedRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void GridCellSizeChangedHandler(Size oldSize, Size newSize)
    {
        throw new NotImplementedException();
    }

    public bool CanSeparateOverlappedItems(Dimension dimension)
    {
        throw new NotImplementedException();
    }

    public Point GetDescendantInsertionPoint()
    {
        throw new NotImplementedException();
    }

    private void DeleteSelection()
    {
        DeleteSelectedItemsCommand command = new DeleteSelectedItemsCommand(this, "Delete selected items");
        command.Execute();
    }

    public void MoveItemsBy(IEnumerable<IWhiteboardItem> items, Vector offset)
    {
        throw new NotImplementedException("The move command from the worksheet view model should be reviewed and placed here. For now this method cannot be used.");
    }

    public void SetCopyWhiteboardItemsDelegate(Func<IEnumerable<IWhiteboardItem>, bool> copyWhiteboardItems)
    {
        _copyWhiteboardItems = copyWhiteboardItems;
    }

    public void ActivateWhiteboard(bool isActive)
    {
    }

    public void SetInsertionPointWhenLoaded(Point insertionPoint)
    {
        throw new NotImplementedException();
    }

    public void HandleItemGotFocus(IWhiteboardItem item)
    {
        ActiveItem = item;
    }

    private void FireActivated()
    {
        Activated?.Invoke();
    }

    public void ClearVisualState()
    {
        ActiveItem = null;
        ClearSelection();
    }

    public void UnSelectItems(IEnumerable<IWhiteboardItem> itemsToRemove)
    {
        if (_selectedItems.Any())
        {
            _selectedItems.RemoveRange(itemsToRemove);
        }
    }

    public void ClearSelection()
    {
        if (_selectedItems.Count() > 0)
        {
            _selectedItems.Clear();
        }
    }

    private static IWorksheetItemManager WorksheetItemManager => WorksheetViewModel.ItemManager;

    IContainerItemManager IWhiteboardItemContainer.ContainerItemManager => WorksheetItemManager;

    IWhiteboardItemManager IWhiteboardControl.WhiteboardItemManager => WorksheetItemManager;

    IUIElementArrangeInfoProvider IHasChildArrangeInfoProvider.ChildArrangeInfoProvider => WorksheetItemManager;

    public Point DefaultInsertionPoint => default;

    public bool IsCrosshairVisible
    {
        get => ActiveOrSelectedItems.Count() == 0;
        set => throw new NotImplementedException();
    }

    public WhiteboardItemsCollection WhiteboardItems => _model.WhiteboardItems;

    public ObservableRangeCollection<IWorksheetPageBreak> PageBreaks => _model.PageBreaks;

    public Size GridCellSize
    {
        get => _worksheetGrid.GridCellSize;
        set => throw new NotImplementedException();
    }

    public bool CanAddItems => true;

    public INotifyCollectionRangeChanged<IWhiteboardItem> SelectedItems => _selectedItems;

    public bool IsTabNavigatable => throw new NotImplementedException();

    public bool ForbidGrowingMovementRight
    {
        set => throw new NotImplementedException();
    }

    public Point GetItemLocation(IWhiteboardItem item) => _model.GetItemWorksheetLocation(item);

    public Rect GetItemRect(IWhiteboardItem item) => WorksheetItemManager.GetItemRect(item);

    public Point GetItemLocation(IWorksheetPageBreak pageBreak) => _model.GetItemWorksheetLocation(pageBreak);

    public void SetItemLocation(IWorksheetPageBreak pageBreak, Point location) => _model.SetItemWorksheetLocation(pageBreak, location);

    public void SaveResizeInformation() => throw new NotImplementedException();

    public void RestoreResizeInformation() => throw new NotImplementedException();

    public void SeparateOverlappedItems(Dimension dimension) => throw new NotImplementedException();

    internal abstract partial class WorksheetContentCommand : global::Quire.Undo.IUndoableCommand
    {
        protected WorksheetControlContent WorksheetContent => _worksheetContent as WorksheetControlContent;

        public string Name { get; private set; }

        protected WorksheetContentCommand(IWorksheetContent worksheetContent, string name)
        {
            Name = name;
            _worksheetContent = worksheetContent;
        }

        public bool Execute()
        {
            return global::Quire.Undo.CommandBuffer.Execute(this);
        }
    }
}

