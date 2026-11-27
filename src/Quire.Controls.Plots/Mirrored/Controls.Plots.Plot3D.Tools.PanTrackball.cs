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
        Point3D? captureStartHit = PerformHitTest(captureStartPosition);
        Point3D? currentHit = PerformHitTest(current);
        bool flag = DebugTools.AssertNotNull(captureStartHit, null).Passed && DebugTools.AssertNotNull(currentHit, null).Passed;
        if (flag)
        {
            ScaleAndAddTranslation(currentHit.Value - captureStartHit.Value);
            TrackingHelper.SetTrackingPoint2D(captureStartPosition).SetTrackingPoint2D(current, 1).SetTrackingLine2D(captureStartPosition, current);
        }
        return flag;
    }

    protected override int GetNumTicksForScaleClick()
    {
        return 0;
    }
}
