using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Whiteboard;
using Quire.Core;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class HeaderFooterZombieViewModel
{
    public VisualBrush ZombieBackground
    {
        get
        {
            return _zombieBackground;
        }
        private set
        {
            _zombieBackground = value;
            OnPropertyChanged("ZombieBackground");
        }
    }

    private static IControlFactory ControlCreator => WorksheetViewModel.ControlCreator;

    private static IWhiteboardItemManager ItemManager => WhiteboardViewModel.WhiteboardItemMgr;

    public IMaybeHasPointThatSnapsToPixel SnapsToPixelItem => _headerFooter;

    private IPageNumberControl ZombiePageNumberControl
    {
        get
        {
            if (DebugTools.DevAssert(ZombieItems.Count == 1 && ZombieItems.First() is IPageNumberControl, "Theres should be one item in ZombieItems and it should be a PageNumberControl").Passed)
            {
                return ZombieItems.First() as IPageNumberControl;
            }
            return null;
        }
    }

    public TranslateTransform ItemToZombieTransform => new TranslateTransform(_headerFooter.Margin.Left, _headerFooter.Margin.Top);

    private bool InitZombieBackground()
    {
        Size bodySize = _headerFooter.BodySize;
        if (!double.IsNaN(bodySize.Width))
        {
            bodySize = _headerFooter.BodySize;
            if (!double.IsNaN(bodySize.Height))
            {
                ZombieBackground = GetVisualBrush(_headerFooter);
                zombieIsInitialized?.Invoke();
                return true;
            }
        }
        return false;
    }

    private void OnHeaderFooterBodySizeChanged(object sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == "BodySize" && InitZombieBackground())
        {
            _headerFooter.PropertyChanged -= OnHeaderFooterBodySizeChanged;
        }
    }

    public void ResetVisualBrush()
    {
        if (ZombieBackground != null)
        {
            _visual = ZombieBackground.Visual;
            ZombieBackground.Visual = null;
        }
    }

    public void RestoreVisualBrush()
    {
        if (ZombieBackground != null && _visual != null)
        {
            ZombieBackground.Visual = _visual;
        }
    }

    private void HookupHeaderFooterEvents()
    {
        _headerFooter.WhiteboardItems.RangeChanged += HandleHeaderFooterItemsChanged;
        _headerFooter.PageNumberControlMoved += HandlePageNumberControlMoved;
        _headerFooter.PageNumberControlActivated += HandlePageNumberControlActivated;
    }

    protected override void OnDispose()
    {
        _headerFooter.WhiteboardItems.RangeChanged -= HandleHeaderFooterItemsChanged;
        _headerFooter.PageNumberControlMoved -= HandlePageNumberControlMoved;
        _headerFooter.PageNumberControlActivated -= HandlePageNumberControlActivated;
        _headerFooter.WhiteboardItems.OfType<IPageNumberControl>().Each(RemovePageNumberControlFromZombies);
    }

    private void HandlePageNumberControlActivated(bool isActive)
    {
        if (ZombieItems.Count != 1 || !(ZombieItems.First() is IPageNumberControl))
        {
            return;
        }
        IWhiteboardItem zombiePageNumberControl = ZombiePageNumberControl;
        if (isActive)
        {
            ItemManager.ActivateItem(zombiePageNumberControl);
        }
        else
        {
            ItemManager.DeactivateItem(zombiePageNumberControl);
        }
    }

    private void HandlePageNumberControlResized(IWhiteboardItem pgNumControl, Size newSize)
    {
        if (pgNumControl is FrameworkElement frameworkElement && !double.IsNaN(frameworkElement.Width))
        {
            Size size = new Size(newSize.Width, double.NaN);
            ItemManager.SetSize(ZombiePageNumberControl, size);
        }
    }

    private void HandlePageNumberControlMoved(IPageNumberControl pgNumControl)
    {
        SetZombieLocationAccordingToOriginalControl(ZombiePageNumberControl, pgNumControl);
    }

    private void SetZombieLocationAccordingToOriginalControl(IPageNumberControl zombieControl, IPageNumberControl originalItem)
    {
        Point itemLocation = ItemManager.GetItemLocation(originalItem);
        itemLocation.Offset(_headerFooter.Margin.Left, _headerFooter.Margin.Top + originalItem.VerticalAlignmentOffset);
        ItemManager.SetItemLocation(zombieControl, itemLocation);
    }
}
