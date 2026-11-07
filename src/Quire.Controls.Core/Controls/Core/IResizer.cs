#nullable disable
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IResizer
{
    Point ResizeBy(Vector offset);
    void ResizeEnd(Point currentPosition);
}
