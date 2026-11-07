#nullable disable
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IUIElementArrangeInfoProvider
{
    Size GetRenderSize(object obj);
    Rect TransformBoundsToAncestor(object descendant, object ancestor, Rect bounds);
    Point TranslatePoint(object source, object target, Point sourcePoint);
}
