using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class WorksheetPageControl
{
    public ObservableRangeCollection<IWhiteboardItem> WhiteboardItems => PageBody.WhiteboardItems;

    public IList<IWhiteboardItem> NonOwnedWhiteboardItems => _nonOwnedWhiteboardItems;

    public Point InsertionPoint
    {
        get
        {
            return PageBody.InsertionPoint;
        }
        set
        {
            PageBody.SetInsertionPointWhenLoaded(value);
        }
    }

    public IWhiteboardItem ActiveItem => PageBody.ActiveItem;

    public IWorksheetPageBody PageBody => _viewModel.PageBody;

    public IHeaderFooter Header
    {
        get
        {
            return _viewModel.Header;
        }
        set
        {
            _viewModel.Header = value;
        }
    }

    public IHeaderFooter Footer
    {
        get
        {
            return _viewModel.Footer;
        }
        set
        {
            _viewModel.Footer = value;
        }
    }

    public IHeaderFooterZombie HeaderZombie
    {
        get
        {
            return _viewModel.HeaderZombie;
        }
        set
        {
            _viewModel.HeaderZombie = value;
        }
    }

    public IHeaderFooterZombie FooterZombie
    {
        get
        {
            return _viewModel.FooterZombie;
        }
        set
        {
            _viewModel.FooterZombie = value;
        }
    }

    public IHeaderFooterZombie FirstPageHeaderZombie
    {
        get
        {
            return _viewModel.FirstPageHeaderZombie;
        }
        set
        {
            _viewModel.FirstPageHeaderZombie = value;
        }
    }

    public IHeaderFooterZombie FirstPageFooterZombie
    {
        get
        {
            return _viewModel.FirstPageFooterZombie;
        }
        set
        {
            _viewModel.FirstPageFooterZombie = value;
        }
    }

    public int PageNumber
    {
        get
        {
            return _viewModel.PageNumber;
        }
        set
        {
            _viewModel.PageNumber = value;
        }
    }

    public bool HasPageBreak
    {
        get
        {
            return _viewModel.HasPageBreak;
        }
        set
        {
            _viewModel.HasPageBreak = value;
        }
    }

    public void AddItemAtLocation(IWhiteboardItem item, Point location)
    {
        _viewModel.AddItemAtLocation(item, location);
    }

    public void RemoveItem(IWhiteboardItem item)
    {
        _viewModel.RemoveItem(item);
    }

    public Point GetNavigateOutLocation(Direction direction)
    {
        return _viewModel.GetNavigateOutLocation(direction);
    }

    public void Activate(bool isActive)
    {
        PageBody.ActivateWhiteboard(isActive);
    }

    public void UpdateNonPrintableSpaceIndicator(double maxXLocation)
    {
        _viewModel.UpdateNonPrintableSpaceIndicator(maxXLocation);
    }

    public void RemoveItems(IEnumerable<IWhiteboardItem> items)
    {
        _viewModel.RemoveItems(items);
    }

    public void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e)
    {
        ((DependencyObject)PageBody).SetValue(e.Property, e.NewValue);
    }

    private static void HandleLabelIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WorksheetPageControl worksheetPageControl)
        {
            ((DependencyObject)worksheetPageControl.PageBody).SetValue(e.Property, e.NewValue);
        }
    }
}
