#nullable disable
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Quire.Core;
using Quire.Mirror;

namespace Quire.Controls.Core.Whiteboard;

public partial class SnapToPixelWhiteboardHelper : ISnapToPixelHelper
{
    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Core.Whiteboard.SnapToPixelWhiteboardHelper+IContainerElement, Ptc.Controls.Core")]
    private interface IContainerElement
    {
        UIElement AsElement { get; }

        IWhiteboardItemContainer AsContainer { get; }
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Core.Whiteboard.SnapToPixelWhiteboardHelper+ContainerElement`1, Ptc.Controls.Core")]
    private class ContainerElement<T> : IContainerElement where T : UIElement, IWhiteboardItemContainer
    {
        private readonly T _t;

        public ContainerElement(T t)
        {
            _t = t;
        }

        public UIElement AsElement => _t;

        public IWhiteboardItemContainer AsContainer => _t;
    }

    private readonly IContainerElement _item;

    public static SnapToPixelWhiteboardHelper Create<T>(T item) where T : UIElement, IWhiteboardItemContainer
    {
        return new SnapToPixelWhiteboardHelper(new ContainerElement<T>(item));
    }

    private SnapToPixelWhiteboardHelper(IContainerElement item)
    {
        _item = item;
    }

    public GeneralTransform TransformFromPoint
    {
        get
        {
            IMaybeHasPointThatSnapsToPixel maybeHasPointThatSnapsToPixel = _item.AsContainer.WhiteboardItems.OfType<IMaybeHasPointThatSnapsToPixel>().FirstOrDefault((IMaybeHasPointThatSnapsToPixel snappedItem) => snappedItem.HasPointThatSnapsToPixel());
            if (maybeHasPointThatSnapsToPixel != null)
            {
                return this.CreateTransformFromChildPoint(maybeHasPointThatSnapsToPixel.SnappingHelper);
            }
            return null;
        }
    }

    public bool HasPointThatSnapsToPixel => _item.AsContainer.WhiteboardItems.OfType<IMaybeHasPointThatSnapsToPixel>().Any((IMaybeHasPointThatSnapsToPixel snappedItem) => snappedItem.HasPointThatSnapsToPixel());

    public UIElement ItemElement => _item.AsElement;
}
