using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Controls.Plots.Plot3D.Tools;

public partial class SpinTrackball : BaseTrackball
{
    private AxisAngleRotation3D _cameraRotation;

    internal static readonly double Radius = Math.Sqrt(2.0);

    private const double RadiusSquared = 2.0;

    private Vector3D _lastCapturedPoint3D;

    private Quaternion _rotationAtCaptureStart;

    private AxisAngleRotation3D CameraRotation => _cameraRotation;

    protected override Transform3D InitializeTransform()
    {
        _cameraRotation = new AxisAngleRotation3D();
        return new RotateTransform3D(_cameraRotation);
    }

    protected override bool TrackStart(Point current, bool useTimer)
    {
        if (!ViewportToTrackball(current, out var result))
        {
            return false;
        }
        _lastCapturedPoint3D = result;
        _rotationAtCaptureStart = new Quaternion(CameraRotation.Axis, CameraRotation.Angle);
        return true;
    }

    protected override bool Track(Point current)
    {
        if (!ViewportToTrackball(current, out var result))
        {
            return false;
        }
        Vector3D axisOfRotation = BaseTransform.Transform(Vector3D.CrossProduct(_lastCapturedPoint3D, result));
        axisOfRotation.Normalize();
        double num = Vector3D.AngleBetween(_lastCapturedPoint3D, result);
        Quaternion quaternion = new Quaternion(axisOfRotation, -num);
        Quaternion combinedRotation = _rotationAtCaptureStart * quaternion;
        CameraRotation.Axis = combinedRotation.Axis;
        CameraRotation.Angle = combinedRotation.Angle;
        return true;
    }

    protected override bool TrackEnd(Point current)
    {
        return false;
    }

    private bool ViewportToTrackball(Point point, out Vector3D result)
    {
        double num = point.X / (Viewport.ActualWidth / 2.0) - 1.0;
        double y = 1.0 - point.Y / (Viewport.ActualHeight / 2.0);
        double zSquared = 2.0 - num * num - y * y;
        bool isOnTrackball = zSquared >= 0.0;
        result = new Vector3D(num, y, isOnTrackball ? Math.Sqrt(zSquared) : 0.0);
        return isOnTrackball;
    }

    public SpinTrackball()
    {
    }
}
