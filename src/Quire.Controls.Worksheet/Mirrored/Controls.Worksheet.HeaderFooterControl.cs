using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Whiteboard;
using Quire.Core;

namespace Quire.Controls.Worksheet;

public partial class HeaderFooterControl
{
    public int PageNumber
    {
        get
        {
            return (int)GetValue(PageNumberProperty);
        }
        set
        {
            SetValue(PageNumberProperty, value);
        }
    }

    public bool IsHeader => Role.IsHeaderRole();

    public bool IsFooter => Role.IsFooterRole();

    public TextFormat TextFormat
    {
        get
        {
            return (TextFormat)GetValue(TextFormatProperty);
        }
        set
        {
            SetValue(TextFormatProperty, value);
        }
    }

    Thickness IHeaderFooterControl.Margin => Margin;

    public Size BodySize
    {
        get
        {
            return new Size(Width, Height);
        }
    }

    private IHeaderFooterViewModel HFViewModel => ViewModel as IHeaderFooterViewModel;

    public ICommonContentsContainer CommonContents => HeaderFooterContainer.CommonItemsCanvas;

    private static void HandleMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        HeaderFooterControl headerFooterControl = d as HeaderFooterControl;
        if (headerFooterControl == null)
        {
            return;
        }
        headerFooterControl.HFViewModel.Margin = (Thickness)e.NewValue;
    }

    private static void HandleBodySizeChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
    {
        DebugTools.DevAssert(args.Property.Name == "Width" || args.Property.Name == "Height", null);
        if (depObj is HeaderFooterControl headerFooterControl)
        {
            headerFooterControl.FirePropertyChanged("BodySize");
        }
    }

    public void HandleDirectionalMove(Direction direction)
    {
        NavigateOutOfHeaderFooter(direction, arg2: false);
    }

    public void MoveItemsBy(IEnumerable<IWhiteboardItem> items, Vector offset)
    {
        WhiteboardViewModel.MoveItemsCommand command = new WhiteboardViewModel.MoveItemsCommand(ViewModel, offset, allowGrowthDownOrRight: false, items);
        command.Execute();
    }

    public bool HandleDelete()
    {
        return HFViewModel.WorksheetHandleDelete();
    }

    public void UpdateItemLocationsAfterGridSizeChanged(Size oldGridCellSize)
    {
        HFViewModel.UpdateItemLocationsAfterGridSizeChanged(oldGridCellSize);
    }

    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    protected override void ViewModelPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        base.ViewModelPropertyChangedHandler(sender, e);
        if (e.PropertyName == "ActiveDescendant" || e.PropertyName == "SelectedDescendants")
        {
            FirePropertyChanged(e.PropertyName);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        ViewModel.PropertyChanged -= ViewModelPropertyChangedHandler;
    }

    public bool IsAssociatedWithPage(IWorksheetPage worksheetPage)
    {
        if (worksheetPage == null)
        {
            return false;
        }
        return worksheetPage.PageNumber == PageNumber;
    }
}
