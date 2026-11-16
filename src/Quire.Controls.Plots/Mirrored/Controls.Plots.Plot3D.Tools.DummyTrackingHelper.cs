#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Controls.Plots.Plot3D.Tools;

internal partial class DummyTrackingHelper
{
    public void Attach()
    {
    }

    public void Detach()
    {
    }

    public void TrackStart()
    {
    }

    public void TrackEnd()
    {
    }

    public ITrackingHelper SetTrackingPoint(Point3D point, int colorId = 0)
    {
        return this;
    }

    public ITrackingHelper AddTrackingPoint(Point3D point, int colorId = 0)
    {
        return this;
    }

    public ITrackingHelper SetTrackingLine(Point3D from, Point3D to, int colorId = 0)
    {
        return this;
    }

    public ITrackingHelper AddTrackingLine(Point3D from, Point3D to, int colorId = 0)
    {
        return this;
    }

    public ITrackingHelper SetTrackingPoint2D(Point point, int colorId = 0)
    {
        return this;
    }

    public ITrackingHelper AddTrackingPoint2D(Point point, int colorId = 0)
    {
        return this;
    }

    public ITrackingHelper SetTrackingLine2D(Point from, Point to, int colorId = 0)
    {
        return this;
    }

    public ITrackingHelper AddTrackingLine2D(Point from, Point to, int colorId = 0)
    {
        return this;
    }

    public void PerformHitTest(Point position, ref Point3D? initialHit)
    {
    }

    public void Reset()
    {
    }

    public DummyTrackingHelper()
    {
    }
}
