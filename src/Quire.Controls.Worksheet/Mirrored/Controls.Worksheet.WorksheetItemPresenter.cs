using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

internal partial class WorksheetItemPresenter
{
    Geometry IWhiteboardItemPresenter.Clip
    {
        get => Clip;
        set => Clip = value;
    }

    public void HandleItemLocationChanged(Point point)
    {
        Canvas.SetLeft(this, point.X);
        Canvas.SetTop(this, point.Y);
    }

    public WorksheetItemPresenter()
    {
    }
}
