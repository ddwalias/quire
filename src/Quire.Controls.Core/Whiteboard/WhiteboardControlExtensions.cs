#nullable disable
using System.Windows;

namespace Quire.Controls.Core.Whiteboard;

public static partial class WhiteboardControlExtensions
{
    public static Rect GetInsertionPointRect(this IWhiteboardControl whiteboard)
    {
        if (whiteboard == null)
        {
            return Rect.Empty;
        }
        Point insertionPoint = whiteboard.InsertionPoint;
        Size gridCellSize = whiteboard.GridCellSize;
        return new Rect(new Point(insertionPoint.X - gridCellSize.Width, insertionPoint.Y - gridCellSize.Height), new Point(insertionPoint.X + gridCellSize.Width, insertionPoint.Y + gridCellSize.Height));
    }
}
