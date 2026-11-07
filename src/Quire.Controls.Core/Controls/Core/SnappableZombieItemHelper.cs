#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using Quire.Core;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class SnappableZombieItemHelper
{
    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Core.SnappableZombieItemHelper+IZombieElement, Ptc.Controls.Core")]
    private interface IZombieElement
    {
        IZombie AsZombie { get; }

        UIElement AsElement { get; }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Core.SnappableZombieItemHelper+ZombieElement`1, Ptc.Controls.Core")]
    private class ZombieElement<T> : IZombieElement where T : UIElement, IZombie
    {
        private readonly T _item;

        public ZombieElement(T item)
        {
            _item = item;
        }

        public IZombie AsZombie => _item;

        public UIElement AsElement => _item;
    }

    private readonly IZombie _zombie;

    private EventHandler _onLayoutUpdated;

    public static SnappableZombieItemHelper Create<T>(T item, bool isPageDescendant) where T : UIElement, IZombie
    {
        return new SnappableZombieItemHelper(new ZombieElement<T>(item), isPageDescendant);
    }

    private SnappableZombieItemHelper(IZombieElement item, bool isPageDescendant)
        : base(item.AsElement, isPageDescendant)
    {
        _zombie = item.AsZombie;
        if (_zombie.SnapsToPixelItem != null)
        {
            _onLayoutUpdated = delegate
            {
                RecalculateRenderTransform();
            };
            ItemElement.LayoutUpdated += _onLayoutUpdated;
        }
    }

    public override void Dispose()
    {
        if (_onLayoutUpdated != null)
        {
            ItemElement.LayoutUpdated -= _onLayoutUpdated;
            _onLayoutUpdated = null;
        }
        base.Dispose();
    }

    protected override Point? RecalculateOffset()
    {
        ISnapToPixelHelper snappingHelper = _zombie.SnapsToPixelItem.SnappingHelper;
        if (!snappingHelper.HasPointThatSnapsToPixel)
        {
            return null;
        }
        GeneralTransform transformFromPoint = snappingHelper.TransformFromPoint;
        Point? point = SnappableItemBaseHelper.CalculateOffset(ItemElement, WpfUtils.GroupTransformsAsFrozen(transformFromPoint, _zombie.ItemToZombieTransform));
        Point? point2 = SnappableItemBaseHelper.CalculateOffset(snappingHelper.ItemElement, transformFromPoint);
        if (!point.HasValue || !point2.HasValue)
        {
            return null;
        }
        return new Point(point.Value.X - point2.Value.X, point.Value.Y - point2.Value.Y);
    }
}
