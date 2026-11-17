using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Core;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class WhiteboardItemZombie
{
    public static IWhiteboardItemManager ItemManager
    {
        get
        {
            if (_itemManager == null || !_itemManager.IsConnected)
            {
                return _itemManager = AppServiceRegistry.GetService<IWhiteboardItemManager>();
            }
            return _itemManager;
        }
    }

    public IWhiteboardItem Item => _item;

    public Size ItemSize { get; set; }

    public Rect ItemBounds { get; set; }

    public double ItemVerticalAlignmentOffset { get; set; }

    public double VerticalAlignmentOffset => 0.0;

    public bool IsEmpty => throw new NotImplementedException();

    public ISnapToPixelHelper SnappingHelper => _snappableHelper;

    public GeneralTransform ItemToZombieTransform
    {
        get
        {
            Point val = ZombieCreator.ConvertPointFromZombieToItem(default, this);
            return new TranslateTransform(val.X, val.Y).Inverse;
        }
    }

    public IMaybeHasPointThatSnapsToPixel SnapsToPixelItem { get; private set; }

    private void HandleZombieVisibilityChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        if (_zombieAdorner != null)
        {
            _zombieAdorner.Visibility = Visibility;
        }
    }

    public void ResetVisualBrush()
    {
        _zombieRect.Fill = null;
        if (_zombieAdorner != null)
        {
            _zombieAdorner.Visual = null;
        }
    }

    public void Hide()
    {
        Visibility = Visibility.Hidden;
    }

    public void Show()
    {
        Visibility = Visibility.Visible;
    }

    public void UpdateZombieRect()
    {
        if (_isDisposed)
        {
            return;
        }
        _zombieRect.Height = ItemBounds.Height;
        _zombieRect.Width = ItemBounds.Width;
        _zombieRect.Fill = ImageUtils.CreateVisualBrush(_itemElement, ItemBounds);
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_zombieRect);
        if (adornerLayer != null)
        {
            adornerLayer.GetAdorners(_zombieRect)?.Each(adornerLayer.Remove);
            Rect box;
            Rect bounds;
            if (Clip == null)
            {
                box = ItemBounds;
                bounds = new Rect(ItemBounds.Size);
            }
            else
            {
                bounds = Clip.Bounds;
                box = new Rect(ZombieCreator.ConvertPointFromZombieToItem(bounds.TopLeft, this), bounds.Size);
            }
            _zombieAdorner = new ZombieAdorner(_zombieRect, ImageUtils.CreateVisualBrush(AdornerLayer.GetAdornerLayer(_itemElement), box), bounds)
            {
                Visibility = Visibility
            };
            adornerLayer.Add(_zombieAdorner);
        }
    }

    public void Activate(bool isActive)
    {
        throw new NotImplementedException();
    }

    public Vector? NavigateIntoItem(Direction direction)
    {
        throw new NotImplementedException();
    }

    public void SimulateMouseClick(Point location, int numberOfClicks)
    {
    }

    public void Dispose()
    {
        DependencyPropertyChangeManager.RemovePropertyChangedHandler(UIElement.VisibilityProperty, this, HandleZombieVisibilityChanged);
        _snappableHelper = null;
        _isDisposed = true;
    }

    public string GetHelpContext()
    {
        DebugTools.FailAssertion("Zombie items shouldn't expose help context.");
        return null;
    }
}
