#nullable disable
#pragma warning disable

namespace Quire.Core;

partial interface ISnapToPixelHelper
{
    global::System.Windows.Media.GeneralTransform TransformFromPoint { get; }
    bool HasPointThatSnapsToPixel { get; }
    global::System.Windows.UIElement ItemElement { get; }
}
