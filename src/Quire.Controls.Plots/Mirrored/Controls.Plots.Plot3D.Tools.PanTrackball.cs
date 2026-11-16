using System.Windows;
using System.Windows.Media.Media3D;
using Quire.Core;

namespace Quire.Controls.Plots.Plot3D.Tools;

public partial class PanTrackball : BasePanZoomTrackball
{
    public PanTrackball()
    {
    }

    protected override bool Track(Point current)
    {
        Point captureStartPosition = CurrentTrackingInfo.CaptureStartPosition;
        Point3D? point1 = PerformHitTest(captureStartPosition);
        Point3D? point2 = PerformHitTest(current);
        bool flag = DebugTools.AssertNotNull(point1, null).Passed && DebugTools.AssertNotNull(point2, null).Passed;
        if (flag)
        {
            ScaleAndAddTranslation(point2.Value - point1.Value);
            TrackingHelper.SetTrackingPoint2D(captureStartPosition).SetTrackingPoint2D(current, 1).SetTrackingLine2D(captureStartPosition, current);
        }
        return flag;
    }

    protected override int GetNumTicksForScaleClick()
    {
        return 0;
    }
}
