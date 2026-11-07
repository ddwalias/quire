#nullable disable
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Core;

public partial interface IWhiteboardItemPresenter
{
    Geometry Clip { get; set; }
    void HandleItemLocationChanged(Point point);
}
