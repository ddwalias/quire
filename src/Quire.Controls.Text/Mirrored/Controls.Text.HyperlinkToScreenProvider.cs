#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Quire.Wpf;

namespace Quire.Controls.Text;

internal partial class HyperlinkToScreenProvider
{
    private readonly RichTextBox _owner;
    private readonly Rect _clickInTextRect;

    public HyperlinkToScreenProvider(RichTextBox owner, TextPointer clickedTextPosition)
    {
        _owner = owner;
        _clickInTextRect = GetRectAroundTextPosition(clickedTextPosition);
    }

    public double ActualHeight => _clickInTextRect.Height;

    public double ActualWidth => _clickInTextRect.Width;

    public bool IsVisible => _owner.IsVisible;

    private Rect GetRectAroundTextPosition(TextPointer clickedPosition)
    {
        Size dpiScale = ImageUtils.DpiScaleForVisual(_owner);
        double minWidth = 10.0 * dpiScale.Width;
        double maxWidth = 100.0 * dpiScale.Width;
        double minHeight = 10.0 * dpiScale.Height;
        double maxHeight = 100.0 * dpiScale.Height;
        Rect rect = clickedPosition.GetCharacterRect(clickedPosition.LogicalDirection);
        if (rect.IsEmpty)
        {
            rect = default;
        }
        else
        {
            TextPointer position = clickedPosition;
            LogicalDirection oppositeDirection = clickedPosition.LogicalDirection == LogicalDirection.Forward
                ? LogicalDirection.Backward
                : LogicalDirection.Forward;
            TextPointer lineBoundary = clickedPosition.LogicalDirection == LogicalDirection.Forward
                ? clickedPosition.GetLineStartPosition(1) ?? clickedPosition.DocumentEnd
                : clickedPosition.GetLineStartPosition(0) ?? clickedPosition.DocumentStart;
            while (rect.Width.IsEqualTo(0.0) && position.CompareTo(lineBoundary) != 0)
            {
                position = position.GetNextInsertionPosition(clickedPosition.LogicalDirection);
                if (position == null)
                {
                    break;
                }
                LogicalDirection direction = position.CompareTo(lineBoundary) != 0
                    ? clickedPosition.LogicalDirection
                    : oppositeDirection;
                rect.Union(position.GetCharacterRect(direction));
            }
        }
        Point topLeft = _owner.PointToScreen(rect.TopLeft);
        Point bottomRight = _owner.PointToScreen(rect.BottomRight);
        Rect screenRect = new Rect(topLeft, bottomRight);
        double verticalExtend = ComputeExtend(screenRect.Height, minHeight, maxHeight);
        double horizontalExtend = ComputeExtend(screenRect.Width, minWidth, maxWidth);
        screenRect.Inflate(horizontalExtend, verticalExtend);
        return new Rect(_owner.PointFromScreen(screenRect.TopLeft), _owner.PointFromScreen(screenRect.BottomRight));
    }

    private static double ComputeExtend(double value, double minRestriction, double maxRestriction) =>
        (value < minRestriction ? minRestriction - value : value > maxRestriction ? maxRestriction - value : 0.0) / 2;

    public Point TranslatePoint(Point point, UIElement relativeTo)
    {
        Point offset = new Point(point.X, point.Y);
        offset.Offset(_clickInTextRect.Left, _clickInTextRect.Top);
        return _owner.TranslatePoint(offset, relativeTo);
    }

    public Point PointToScreen(Point point)
    {
        Point offset = new Point(point.X, point.Y);
        offset.Offset(_clickInTextRect.Left, _clickInTextRect.Top);
        return _owner.PointToScreen(offset);
    }
}
