using System.Windows;

namespace Quire.Controls.Whiteboard;

public partial class PositionComparer
{
    /// <summary>
    /// Worksheet calculation order: top to bottom, then left to right. Positions are compared to two decimals so
    /// regions placed on the same row in device-independent pixels compare equal vertically.
    /// </summary>
    public int Compare(Point positionOne, Point positionTwo)
    {
        int c = Math.Round(positionOne.Y, 2).CompareTo(Math.Round(positionTwo.Y, 2));
        return c != 0 ? c : Math.Round(positionOne.X, 2).CompareTo(Math.Round(positionTwo.X, 2));
    }
}
