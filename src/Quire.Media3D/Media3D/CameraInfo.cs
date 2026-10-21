using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class CameraInfo : Animatable
{
    public static readonly Matrix3D ZeroMatrix = new Matrix3D(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

    public static readonly DependencyProperty NameProperty;

    public static readonly DependencyProperty CameraProperty;

    public static readonly DependencyProperty ViewportWidthProperty;

    public static readonly DependencyProperty ViewportHeightProperty;

    private static readonly DependencyPropertyKey ViewMatrixKey;

    public static readonly DependencyProperty ViewMatrixProperty;

    private static readonly DependencyPropertyKey ProjectionMatrixKey;

    public static readonly DependencyProperty ProjectionMatrixProperty;

    public static readonly DependencyProperty TotalTransformProperty;

    public static readonly DependencyProperty InverseTransformProperty;

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public Camera Camera
    {
        get => (Camera)GetValue(CameraProperty);
        set => SetValue(CameraProperty, value);
    }

    public double ViewportWidth
    {
        get => (double)GetValue(ViewportWidthProperty);
        set => SetValue(ViewportWidthProperty, value);
    }

    public double ViewportHeight
    {
        get => (double)GetValue(ViewportHeightProperty);
        set => SetValue(ViewportHeightProperty, value);
    }

    public Matrix3D ViewMatrix
    {
        get => (Matrix3D)GetValue(ViewMatrixProperty);
        private set => SetValue(ViewMatrixKey, value);
    }

    public Matrix3D ProjectionMatrix
    {
        get => (Matrix3D)GetValue(ProjectionMatrixProperty);
        private set => SetValue(ProjectionMatrixKey, value);
    }

    public Matrix3D TotalTransform
    {
        get => (Matrix3D)GetValue(TotalTransformProperty);
        protected set => SetValue(TotalTransformProperty, value);
    }

    public Matrix3D InverseTransform
    {
        get => (Matrix3D)GetValue(InverseTransformProperty);
        protected set => SetValue(InverseTransformProperty, value);
    }

    private static void CameraPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        CameraInfo cameraInfo = obj as CameraInfo;
        cameraInfo.ViewMatrix = GetViewMatrix(cameraInfo.Camera);
        ViewportPropertyChanged(obj, args);
    }

    private static void ViewportPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        CameraInfo cameraInfo = obj as CameraInfo;
        cameraInfo.ViewMatrix = GetViewMatrix(cameraInfo.Camera);
        cameraInfo.ProjectionMatrix = GetProjectionMatrix(cameraInfo.Camera, cameraInfo.ViewportWidth / cameraInfo.ViewportHeight);
        cameraInfo.TotalTransform = GetTotalTransform(cameraInfo.Camera, cameraInfo.ViewportWidth / cameraInfo.ViewportHeight);
        cameraInfo.InverseTransform = GetInverseTransform(cameraInfo.Camera, cameraInfo.ViewportWidth / cameraInfo.ViewportHeight);
    }

    public static Matrix3D GetViewMatrix(Camera camera)
    {
        Matrix3D result = Matrix3D.Identity;
        if (camera == null)
        {
            result = ZeroMatrix;
        }
        else if (camera is MatrixCamera)
        {
            result = (camera as MatrixCamera).ViewMatrix;
        }
        else if (camera is ProjectionCamera)
        {
            ProjectionCamera projectionCamera = camera as ProjectionCamera;
            Vector3D vector3D = -projectionCamera.LookDirection;
            vector3D.Normalize();
            Vector3D vector3D2 = Vector3D.CrossProduct(projectionCamera.UpDirection, vector3D);
            vector3D2.Normalize();
            Vector3D vector = Vector3D.CrossProduct(vector3D, vector3D2);
            Vector3D vector2 = (Vector3D)projectionCamera.Position;
            result = new Matrix3D(vector3D2.X, vector.X, vector3D.X, 0.0, vector3D2.Y, vector.Y, vector3D.Y, 0.0, vector3D2.Z, vector.Z, vector3D.Z, 0.0, -Vector3D.DotProduct(vector3D2, vector2), -Vector3D.DotProduct(vector, vector2), -Vector3D.DotProduct(vector3D, vector2), 1.0);
        }
        else if (camera != null)
        {
            throw new ApplicationException("ViewMatrix");
        }
        return result;
    }

    public static Matrix3D GetProjectionMatrix(Camera cam, double aspectRatio)
    {
        Matrix3D result = Matrix3D.Identity;
        if (cam == null)
        {
            result = ZeroMatrix;
        }
        else if (cam is MatrixCamera)
        {
            result = (cam as MatrixCamera).ProjectionMatrix;
        }
        else if (cam is OrthographicCamera)
        {
            OrthographicCamera orthographicCamera = cam as OrthographicCamera;
            double num = 2.0 / orthographicCamera.Width;
            double m = num * aspectRatio;
            double nearPlaneDistance = orthographicCamera.NearPlaneDistance;
            double num2 = orthographicCamera.FarPlaneDistance;
            if (double.IsPositiveInfinity(num2))
            {
                num2 = 10000000000.0;
            }
            result = new Matrix3D(num, 0.0, 0.0, 0.0, 0.0, m, 0.0, 0.0, 0.0, 0.0, 1.0 / (nearPlaneDistance - num2), 0.0, 0.0, 0.0, nearPlaneDistance / (nearPlaneDistance - num2), 1.0);
        }
        else if (cam is PerspectiveCamera)
        {
            PerspectiveCamera perspectiveCamera = cam as PerspectiveCamera;
            double num3 = 1.0 / Math.Tan(Math.PI * perspectiveCamera.FieldOfView / 360.0);
            double m2 = num3 * aspectRatio;
            double nearPlaneDistance2 = perspectiveCamera.NearPlaneDistance;
            double farPlaneDistance = perspectiveCamera.FarPlaneDistance;
            double num4 = ((farPlaneDistance == double.PositiveInfinity) ? (-1.0) : (farPlaneDistance / (nearPlaneDistance2 - farPlaneDistance)));
            double offsetZ = nearPlaneDistance2 * num4;
            result = new Matrix3D(num3, 0.0, 0.0, 0.0, 0.0, m2, 0.0, 0.0, 0.0, 0.0, num4, -1.0, 0.0, 0.0, offsetZ, 0.0);
        }
        else if (cam != null)
        {
            throw new ApplicationException("ProjectionMatrix");
        }
        return result;
    }

    public static Matrix3D GetTotalTransform(Camera cam, double aspectRatio)
    {
        Matrix3D result = Matrix3D.Identity;
        if (cam == null)
        {
            result = ZeroMatrix;
        }
        else
        {
            if (cam.Transform != null)
            {
                Matrix3D value = cam.Transform.Value;
                if (!value.HasInverse)
                {
                    result = ZeroMatrix;
                }
                else
                {
                    value.Invert();
                    result.Append(value);
                }
            }
            result.Append(GetViewMatrix(cam));
            result.Append(GetProjectionMatrix(cam, aspectRatio));
        }
        return result;
    }

    public static Matrix3D GetInverseTransform(Camera cam, double aspectRatio)
    {
        Matrix3D matrix3D = GetTotalTransform(cam, aspectRatio);
        if (!(matrix3D == ZeroMatrix))
        {
            if (!matrix3D.HasInverse)
            {
                matrix3D = ZeroMatrix;
            }
            else
            {
                matrix3D.Invert();
            }
        }
        return matrix3D;
    }

    public override string ToString()
    {
        return string.Format("View Matrix: {0}\nProjection Matrix: {1}\nTotal Transform: {2}", ViewMatrix, ProjectionMatrix, TotalTransform);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new CameraInfo();
    }

    public CameraInfo()
    {
    }

    static CameraInfo()
    {
        NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(CameraInfo));
        CameraProperty = DependencyProperty.Register("Camera", typeof(Camera), typeof(CameraInfo), new PropertyMetadata(null, new PropertyChangedCallback(CameraPropertyChanged)));
        ViewportWidthProperty = DependencyProperty.Register("ViewportWidth", typeof(double), typeof(CameraInfo), new PropertyMetadata(1.0, new PropertyChangedCallback(ViewportPropertyChanged)));
        ViewportHeightProperty = DependencyProperty.Register("ViewportHeight", typeof(double), typeof(CameraInfo), new PropertyMetadata(1.0, new PropertyChangedCallback(ViewportPropertyChanged)));
        ViewMatrixKey = DependencyProperty.RegisterReadOnly("ViewMatrix", typeof(Matrix3D), typeof(CameraInfo), new PropertyMetadata(default(Matrix3D)));
        ViewMatrixProperty = ViewMatrixKey.DependencyProperty;
        ProjectionMatrixKey = DependencyProperty.RegisterReadOnly("ProjectionMatrix", typeof(Matrix3D), typeof(CameraInfo), new PropertyMetadata(default(Matrix3D)));
        ProjectionMatrixProperty = ProjectionMatrixKey.DependencyProperty;
        TotalTransformProperty = DependencyProperty.Register("TotalTransform", typeof(Matrix3D), typeof(CameraInfo));
        InverseTransformProperty = DependencyProperty.Register("InverseTransform", typeof(Matrix3D), typeof(CameraInfo));
    }
}
