using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class Twister : AlgorithmicTransform
{
    private AxisAngleRotation3D axisRotate;

    private RotateTransform3D xform;

    public static readonly DependencyProperty AxisProperty;

    public static readonly DependencyProperty CenterProperty;

    public static readonly DependencyProperty AngleProperty;

    public static readonly DependencyProperty AttenuationProperty;

    public Vector3D Axis
    {
        get => (Vector3D)GetValue(AxisProperty);
        set => SetValue(AxisProperty, value);
    }

    public Point3D Center
    {
        get => (Point3D)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    public double Angle
    {
        get => (double)GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }

    public double Attenuation
    {
        get => (double)GetValue(AttenuationProperty);
        set => SetValue(AttenuationProperty, value);
    }

    public Twister()
    {
        axisRotate = new AxisAngleRotation3D();
        xform = new RotateTransform3D(axisRotate);
    }

    private static bool ValidateAttenuation(object obj)
    {
        return (double)obj > 0.0;
    }

    protected override Freezable CreateInstanceCore()
    {
        return new Twister();
    }

    public override void Transform(Point3DCollection points)
    {
        xform.CenterX = Center.X;
        xform.CenterY = Center.Y;
        xform.CenterZ = Center.Z;
        axisRotate.Axis = Axis;
        Vector3D axis = Axis;
        axis.Normalize();
        double num = Angle / Attenuation;
        for (int i = 0; i < points.Count; i++)
        {
            axisRotate.Angle = num * Vector3D.DotProduct(axis, points[i] - Center);
            points[i] = xform.Transform(points[i]);
        }
    }

    static Twister()
    {
        AxisProperty = DependencyProperty.Register("Axis", typeof(Vector3D), typeof(Twister), new PropertyMetadata(new Vector3D(0.0, 1.0, 0.0)));
        CenterProperty = DependencyProperty.Register("Center", typeof(Point3D), typeof(Twister), new PropertyMetadata(new Point3D(0.0, 0.0, 0.0)));
        AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(Twister), new PropertyMetadata(0.0));
        AttenuationProperty = DependencyProperty.Register("Attenuation", typeof(double), typeof(Twister), new PropertyMetadata(1.0), new ValidateValueCallback(ValidateAttenuation));
    }
}
