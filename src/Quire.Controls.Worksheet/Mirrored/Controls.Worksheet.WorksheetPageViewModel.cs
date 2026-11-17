using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;

namespace Quire.Controls.Worksheet;

public partial class WorksheetPageViewModel
{
    public IWorksheetPageBody PageBody { get; private set; }

    public IHeaderFooterZombie HeaderZombie { get; set; }

    public IHeaderFooterZombie FooterZombie { get; set; }

    public IHeaderFooterZombie FirstPageHeaderZombie { get; set; }

    public IHeaderFooterZombie FirstPageFooterZombie { get; set; }

    public IHeaderFooter Header
    {
        get
        {
            return _header;
        }
        set
        {
            if (_header == value)
            {
                return;
            }
            IWhiteboardControl whiteboardControl = _header as IWhiteboardControl;
            IWhiteboardControl whiteboardControl2 = value as IWhiteboardControl;
            if (whiteboardControl != null)
            {
                whiteboardControl.RequestScrollTo -= OnSectionRequestScrollTo;
            }
            if (whiteboardControl2 != null)
            {
                whiteboardControl2.RequestScrollTo += OnSectionRequestScrollTo;
            }
            _header = value;
            OnPropertyChanged("Header");
        }
    }

    public IHeaderFooter Footer
    {
        get
        {
            return _footer;
        }
        set
        {
            if (_footer == value)
            {
                return;
            }
            IWhiteboardControl whiteboardControl = _footer as IWhiteboardControl;
            IWhiteboardControl whiteboardControl2 = value as IWhiteboardControl;
            if (whiteboardControl != null)
            {
                whiteboardControl.RequestScrollTo -= OnSectionRequestScrollTo;
            }
            if (whiteboardControl2 != null)
            {
                whiteboardControl2.RequestScrollTo += OnSectionRequestScrollTo;
            }
            _footer = value;
            OnPropertyChanged("Footer");
        }
    }

    public int PageNumber
    {
        get
        {
            return _pageNumber;
        }
        set
        {
            if (_pageNumber == value)
            {
                return;
            }
            _pageNumber = value;
            OnPropertyChanged("PageNumber");
        }
    }

    public bool HasPageBreak
    {
        get
        {
            return _hasPageBreak;
        }
        set
        {
            if (_hasPageBreak != value)
            {
                _hasPageBreak = value;
                OnPropertyChanged("HasPageBreak");
            }
        }
    }

    public bool ShowNonPrintableSpaceIndicator
    {
        get
        {
            return _showNonPrintableSpaceIndicator;
        }
        set
        {
            if (_showNonPrintableSpaceIndicator != value)
            {
                _showNonPrintableSpaceIndicator = value;
                OnPropertyChanged("ShowNonPrintableSpaceIndicator");
            }
        }
    }

    public double NonPrintableSpaceButtonPosition
    {
        get
        {
            return _nonPrintableSpaceButtonPosition;
        }
        set
        {
            _nonPrintableSpaceButtonPosition = value;
            OnPropertyChanged("NonPrintableSpaceButtonPosition");
        }
    }

    public double NonPrintableSpaceIndicatorHeight
    {
        get
        {
            return _nonPrintableSpaceIndicatorHeight;
        }
        set
        {
            _nonPrintableSpaceIndicatorHeight = value;
            OnPropertyChanged("NonPrintableSpaceIndicatorHeight");
        }
    }

    public void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs args)
    {
        IWorksheetPageSection arg = args.OriginalSource as IWorksheetPageSection;
        if (arg == null)
        {
            return;
        }
        Rect arg2 = args.TargetRect;
        if (arg2.IsEmpty && args.OriginalSource is IWhiteboardControl whiteboardControl && whiteboardControl.SelectedItems.IsNullOrEmpty())
        {
            arg2 = whiteboardControl.GetInsertionPointRect();
        }
        RequestScrollTo(arg, arg2);
        args.Handled = true;
    }

    public Point GetNavigateOutLocation(Direction direction)
    {
        return PageBody.GetNavigationStartLocation(direction);
    }

    private void OnSectionRequestScrollTo(IWhiteboardControl section, Rect rect)
    {
        RequestScrollTo(section as IWorksheetPageSection, rect);
    }

    protected override void OnDispose()
    {
        Header = null;
        Footer = null;
        PageBody.RequestScrollTo -= OnSectionRequestScrollTo;
        PageBody.Activated -= PageBodyActivated;
        PageBody.Dispose();
        if (HeaderZombie != null)
        {
            HeaderZombie.Dispose();
            HeaderZombie = null;
        }
        if (FooterZombie != null)
        {
            FooterZombie.Dispose();
            FooterZombie = null;
        }
        if (FirstPageHeaderZombie != null)
        {
            FirstPageHeaderZombie.Dispose();
            FirstPageHeaderZombie = null;
        }
        if (FirstPageFooterZombie != null)
        {
            FirstPageFooterZombie.Dispose();
            FirstPageFooterZombie = null;
        }
        base.OnDispose();
    }

    public void AddItemAtLocation(IWhiteboardItem item, Point location)
    {
        if (!PageBody.WhiteboardItems.Contains(item))
        {
            PageBody.AddItemAtLocation(item, location);
            if (WorksheetViewModel.ItemManager.IsItemSelected(item))
            {
                PageBody.SelectItem(item);
            }
            return;
        }
        if (PageBody.GetItemLocation(item) == location)
        {
            return;
        }
        PageBody.SetItemLocation(item, location);
    }

    public void RemoveItem(IWhiteboardItem item)
    {
        DebugTools.DevAssert(PageBody.WhiteboardItems.Contains(item), "The page does not contain the item which is supposed to be removed from it.");
        RemoveItems(new IWhiteboardItem[1] { item });
    }

    public void RemoveItems(IEnumerable<IWhiteboardItem> items)
    {
        foreach (IWhiteboardItem item in items)
        {
            if (item is IWhiteboardItemZombie whiteboardItemZombie)
            {
                whiteboardItemZombie.ResetVisualBrush();
            }
        }
        PageBody.UnSelectItems(items);
        PageBody.WhiteboardItems.RemoveRange(items);
    }
}
