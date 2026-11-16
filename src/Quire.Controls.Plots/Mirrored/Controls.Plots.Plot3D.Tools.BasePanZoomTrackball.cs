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
        Vector3D vector3D2 = translation * CurrentScale;
        Vector3D vector3D3 = vector3D - vector3D2;
        TranslateTransform.OffsetX = vector3D3.X;
        TranslateTransform.OffsetY = vector3D3.Y;
        TranslateTransform.OffsetZ = vector3D3.Z;
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
        Point3D? point3D2 = PerformHitTest(new Point(Viewport.ActualWidth / 2.0, Viewport.ActualHeight / 2.0));
        if (DebugTools.AssertNotNull(point3D2, null).Passed && DebugTools.AssertNotNull(point3D, null).Passed)
        {
            Point3D value = point3D.Value;
            Point3D value2 = point3D2.Value;
            TrackingHelper.AddTrackingPoint(value).AddTrackingPoint(value2, 1).AddTrackingLine(value, value2);
            ScaleAndAddTranslation(value2 - value);
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
