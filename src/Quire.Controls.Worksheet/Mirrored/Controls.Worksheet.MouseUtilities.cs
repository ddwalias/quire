using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Worksheet;

public partial class MouseUtilities
{
    private partial struct Win32Point
    {
        public int X;
        public int Y;
    }

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(ref Win32Point pt);

    public static Point GetMousePosition(Visual relativeTo)
    {
        Win32Point pt = default;
        GetCursorPos(ref pt);
        return relativeTo.PointFromScreen(new Point(pt.X, pt.Y));
    }

    public MouseUtilities() { }
}
