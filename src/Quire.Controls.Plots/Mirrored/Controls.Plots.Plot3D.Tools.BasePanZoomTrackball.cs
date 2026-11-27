using System.Windows;
using System.Windows.Media.Media3D;
using Quire.Media3D;

namespace Quire.Controls.Plots.Plot3D.Tools;

public abstract partial class BasePanZoomTrackball : BaseTrackball
{
    protected TranslateTransform3D TranslateTransform { get; private set; }

    protected TranslateTransform3D TranslateTransformAtTrackStart { get; private set; }

    protected void ScaleAndAddTranslation(Vector3D translation)
    {
        if (DebugTools.AssertNotNull(TranslateTransformAtTrackStart, null).Failed)
        {
            return;
        }
        Vector3D vector3D = new Vector3D(TranslateTransformAtTrackStart.OffsetX, TranslateTransformAtTrackStart.OffsetY, TranslateTransformAtTrackStart.OffsetZ);
        Vector3D scaledTranslation = translation * CurrentScale;
        Vector3D adjustedTranslation = vector3D - scaledTranslation;
        TranslateTransform.OffsetX = adjustedTranslation.X;
        TranslateTransform.OffsetY = adjustedTranslation.Y;
        TranslateTransform.OffsetZ = adjustedTranslation.Z;
    }

    protected override Transform3D InitializeTransform()
    {
        return TranslateTransform = new TranslateTransform3D();
    }

    protected override bool TrackStart(Point current, bool useTimer)
    {
        TrackingHelper.TrackStart();
        TranslateTransformAtTrackStart = BaseTrackball.CloneAndFreeze(TranslateTransform);
        new Transform3DGroup
        {
            Children = 
            {
                BaseTransform,
                (Transform3D)TranslateTransformAtTrackStart
            }
        };
        return true;
    }

    protected abstract int GetNumTicksForScaleClick();

    protected override bool TrackEnd(Point current)
    {
        DoubleComparer defaultDoubleComparer = DoubleComparer.DefaultDoubleComparer;
        Vector val = current - CurrentTrackingInfo.CaptureStartPosition;
        bool flag = defaultDoubleComparer.Compare(val.Length, 0.0) == 0;
        if (flag)
        {
            int numTicksForScaleClick = GetNumTicksForScaleClick();
            ApplyScaleWithCenter(ScaleFromTicks(numTicksForScaleClick), current);
        }
        TrackingHelper.TrackEnd();
        return flag;
    }

    protected void ApplyScaleWithCenter(double requestedScale, Point screenAreaCenter)
    {
        double currentScale = CurrentScale;
        Point3D? point3D = PerformHitTest(screenAreaCenter);
        AddScale(requestedScale);
        Point3D? viewportCenterPoint = PerformHitTest(new Point(Viewport.ActualWidth / 2.0, Viewport.ActualHeight / 2.0));
        if (DebugTools.AssertNotNull(viewportCenterPoint, null).Passed && DebugTools.AssertNotNull(point3D, null).Passed)
        {
            Point3D hitPoint = point3D.Value;
            Point3D viewportCenterHit = viewportCenterPoint.Value;
            TrackingHelper.AddTrackingPoint(hitPoint).AddTrackingPoint(viewportCenterHit, 1).AddTrackingLine(hitPoint, viewportCenterHit);
            ScaleAndAddTranslation(viewportCenterHit - hitPoint);
        }
        else
        {
            SetScale(currentScale);
        }
    }

    protected Point3D? PerformHitTest(Point position)
    {
        Point3D? initialHit = ViewportInfo.Point2DtoPoint3D(Viewport, position, Model3DBoundingBox);
        TrackingHelper.PerformHitTest(position, ref initialHit);
        return initialHit;
    }

    protected BasePanZoomTrackball()
    {
    }
}
