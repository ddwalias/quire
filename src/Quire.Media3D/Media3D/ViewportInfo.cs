using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Quire.Core;

namespace Quire.Media3D;

public partial class ViewportInfo : Animatable
{
    public static readonly Matrix3D ZeroMatrix = new Matrix3D(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

    public static readonly DependencyProperty Viewport3DProperty;

    private static readonly DependencyPropertyKey TransformKey;

    public static readonly DependencyProperty TransformProperty;

    public Viewport3D Viewport3D
    {
        get => (Viewport3D)GetValue(Viewport3DProperty);
        set => SetValue(Viewport3DProperty, value);
    }

    public Matrix3D Transform
    {
        get => (Matrix3D)GetValue(TransformProperty);
        protected set => SetValue(TransformKey, value);
    }

    private static void Viewport3DChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        (obj as ViewportInfo).Viewport3DChanged(args);
    }

    private void Viewport3DChanged(DependencyPropertyChangedEventArgs args)
    {
        if (Viewport3D == null)
        {
            Transform = ZeroMatrix;
        }
        else
        {
            Transform = CameraInfo.GetTotalTransform(Viewport3D.Camera, Viewport3D.ActualWidth / Viewport3D.ActualHeight);
        }
    }

    public static Matrix3D GetTotalTransform(Viewport3DVisual vis)
    {
        Matrix3D cameraTransform = GetCameraTransform(vis);
        cameraTransform.Append(GetViewportTransform(vis));
        return cameraTransform;
    }

    public static Matrix3D GetTotalTransform(Viewport3D viewport)
    {
        Matrix3D cameraTransform = GetCameraTransform(viewport);
        cameraTransform.Append(GetViewportTransform(viewport));
        return cameraTransform;
    }

    public static Matrix3D GetCameraTransform(Viewport3DVisual vis)
    {
        if (!IsViewportArranged(vis))
        {
            return ZeroMatrix;
        }
        Rect viewport = vis.Viewport;
        double aspectRatio = viewport.Size.Width / viewport.Size.Height;
        return CameraInfo.GetTotalTransform(vis.Camera, aspectRatio);
    }

    public static Matrix3D GetCameraTransform(Viewport3D viewport)
    {
        if (!IsViewportArranged(viewport))
        {
            return ZeroMatrix;
        }
        double aspectRatio = viewport.ActualWidth / viewport.ActualHeight;
        return CameraInfo.GetTotalTransform(viewport.Camera, aspectRatio);
    }

    public static Matrix3D GetViewportTransform(Viewport3DVisual vis)
    {
        if (!IsViewportArranged(vis))
        {
            return ZeroMatrix;
        }
        Rect viewport = vis.Viewport;
        return GetViewportTransform(viewport.Width, viewport.Height, viewport.X, viewport.Y);
    }

    public static Matrix3D GetViewportTransform(Viewport3D viewport)
    {
        if (!IsViewportArranged(viewport))
        {
            return ZeroMatrix;
        }
        return GetViewportTransform(viewport.ActualWidth, viewport.ActualHeight);
    }

    private static Matrix3D GetViewportTransform(double width, double height, double left = 0.0, double top = 0.0)
    {
        return new Matrix3D(width / 2.0, 0.0, 0.0, 0.0, 0.0, -height / 2.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, left + width / 2.0, top + height / 2.0, 0.0, 1.0);
    }

    private static bool IsViewportArranged(Viewport3DVisual vis)
    {
        return !vis.Viewport.IsEmpty;
    }

    private static bool IsViewportArranged(Viewport3D viewport)
    {
        if (viewport.ActualWidth != 0.0)
        {
            return viewport.ActualHeight != 0.0;
        }
        return false;
    }

    public static Point Point3DtoPoint2D(Viewport3D viewport, Point3D point)
    {
        Matrix3D totalTransform = GetTotalTransform(viewport);
        Point3D point2 = totalTransform.Transform(point);
        return new Point(point2.X, point2.Y);
    }

    public static bool Point2DtoPoint3D(Viewport3D viewport, Point ptIn, out LineRange range)
    {
        range = default;
        Point3D point = new Point3D(ptIn.X, ptIn.Y, 0.0);
        Matrix3D viewportTransform = GetViewportTransform(viewport);
        Matrix3D cameraTransform = GetCameraTransform(viewport);
        if (!viewportTransform.HasInverse)
        {
            return false;
        }
        if (!cameraTransform.HasInverse)
        {
            return false;
        }
        viewportTransform.Invert();
        cameraTransform.Invert();
        Point3D point2 = viewportTransform.Transform(point);
        point2.Z = 0.01;
        Point3D point3 = cameraTransform.Transform(point2);
        point2.Z = 0.99;
        Point3D point4 = cameraTransform.Transform(point2);
        range = new LineRange(point3, point4);
        return true;
    }

    public static Point3D? Point2DtoPoint3D(Viewport3D viewport, Point ptIn, Rect3D box)
    {
        Matrix3D viewportTransform = GetViewportTransform(viewport);
        Matrix3D cameraTransform = GetCameraTransform(viewport);
        if (!viewportTransform.HasInverse)
        {
            return null;
        }
        if (!cameraTransform.HasInverse)
        {
            return null;
        }
        double zValueForHitTesting = GetZValueForHitTesting(viewport.Camera, cameraTransform, box);
        viewportTransform.Invert();
        cameraTransform.Invert();
        Point3D point = viewportTransform.Transform(new Point3D(ptIn.X, ptIn.Y, zValueForHitTesting));
        return cameraTransform.Transform(point);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new ViewportInfo();
    }

    public static double GetZValueForHitTesting(Camera camera, Matrix3D matxCamera, Rect3D box)
    {
        double z = matxCamera.Transform(new Point3D(box.X, box.Y, box.Z)).Z;
        double z2 = matxCamera.Transform(new Point3D(box.X, box.Y + box.SizeY, box.Z)).Z;
        double z3 = matxCamera.Transform(new Point3D(box.X + box.SizeX, box.Y, box.Z)).Z;
        double z4 = matxCamera.Transform(new Point3D(box.X + box.SizeX, box.Y + box.SizeY, box.Z)).Z;
        double z5 = matxCamera.Transform(new Point3D(box.X, box.Y, box.Z + box.SizeZ)).Z;
        double z6 = matxCamera.Transform(new Point3D(box.X, box.Y + box.SizeY, box.Z + box.SizeZ)).Z;
        double z7 = matxCamera.Transform(new Point3D(box.X + box.SizeX, box.Y, box.Z + box.SizeZ)).Z;
        double z8 = matxCamera.Transform(new Point3D(box.X + box.SizeX, box.Y + box.SizeY, box.Z + box.SizeZ)).Z;
        double result = ((camera is PerspectiveCamera) ? Math.Min(z, Math.Min(z2, Math.Min(z3, Math.Min(z4, Math.Min(z5, Math.Min(z6, Math.Min(z7, z8))))))) : Math.Max(z, Math.Max(z2, Math.Max(z3, Math.Max(z4, Math.Max(z5, Math.Max(z6, Math.Max(z7, z8))))))));
        DebugTools.DevAssert(camera is PerspectiveCamera || camera is OrthographicCamera, null);
        return result;
    }

    public ViewportInfo()
    {
    }

    static ViewportInfo()
    {
        Viewport3DProperty = DependencyProperty.Register("Viewport3D", typeof(Viewport3D), typeof(ViewportInfo), new PropertyMetadata(null, new PropertyChangedCallback(Viewport3DChanged)));
        TransformKey = DependencyProperty.RegisterReadOnly("Transform", typeof(Matrix3D), typeof(ViewportInfo), new PropertyMetadata(default(Matrix3D)));
        TransformProperty = TransformKey.DependencyProperty;
    }
}
