using System.Windows;
using Quire.Core;

namespace Quire.Controls.Whiteboard;

public partial class PositionComparer
{
    public int Compare(Point positionOne, Point positionTwo)
    {
        int result = DoubleComparer.DefaultDoubleComparer.Compare(positionOne.Y, positionTwo.Y);
        if (result == 0)
        {
            result = DoubleComparer.DefaultDoubleComparer.Compare(positionOne.X, positionTwo.X);
        }
        if (result == 0)
        {
            result = -1;
        }
        return result;
    }
}
