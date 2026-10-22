#nullable disable
#pragma warning disable

namespace Quire.Core;

partial interface ISnappableItemHelper
{
    bool IsPageDescendant { get; }
    global::System.Windows.UIElement RenderTarget { get; }
}
