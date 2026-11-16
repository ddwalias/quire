using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Controls.Plots.Plot3D.Tools;

internal partial interface ITrackingHelper
{
    void Attach();

    void Detach();

    void Reset();

    void TrackStart();

    void TrackEnd();

    ITrackingHelper SetTrackingPoint(Point3D point, int colorId = 0);

    ITrackingHelper AddTrackingPoint(Point3D point, int colorId = 0);

    ITrackingHelper SetTrackingLine(Point3D from, Point3D to, int colorId = 0);

    ITrackingHelper AddTrackingLine(Point3D from, Point3D to, int colorId = 0);

    ITrackingHelper SetTrackingPoint2D(Point point, int colorId = 0);

    ITrackingHelper AddTrackingPoint2D(Point point, int colorId = 0);

    ITrackingHelper SetTrackingLine2D(Point from, Point to, int colorId = 0);

    ITrackingHelper AddTrackingLine2D(Point from, Point to, int colorId = 0);

    void PerformHitTest(Point position, ref Point3D? initialHit);
}
