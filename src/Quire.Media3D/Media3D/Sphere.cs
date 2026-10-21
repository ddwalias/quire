using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class Sphere : ModelVisualBase
{
    private AxisAngleRotation3D rotate;
    private RotateTransform3D xform;

    public static readonly DependencyProperty SlicesProperty =
        DependencyProperty.Register("Slices", typeof(int), typeof(Sphere),
            new PropertyMetadata(36, ModelVisualBase.PropertyChanged), ValidatePositive);

    public static readonly DependencyProperty StacksProperty =
        DependencyProperty.Register("Stacks", typeof(int), typeof(Sphere),
            new PropertyMetadata(18, ModelVisualBase.PropertyChanged), ValidatePositive);

    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.Register("Center", typeof(Point3D), typeof(Sphere),
            new PropertyMetadata(new Point3D(0.0, 0.0, 0.0), ModelVisualBase.PropertyChanged));

    public static readonly DependencyProperty LongitudeFromProperty =
        DependencyProperty.Register("LongitudeFrom", typeof(double), typeof(Sphere),
            new PropertyMetadata(-180.0, ModelVisualBase.PropertyChanged), ValidateLongitude);

    public static readonly DependencyProperty LongitudeToProperty =
        DependencyProperty.Register("LongitudeTo", typeof(double), typeof(Sphere),
            new PropertyMetadata(180.0, ModelVisualBase.PropertyChanged), ValidateLongitude);

    public static readonly DependencyProperty LatitudeFromProperty =
        DependencyProperty.Register("LatitudeFrom", typeof(double), typeof(Sphere),
            new PropertyMetadata(90.0, ModelVisualBase.PropertyChanged), ValidateLatitude);

    public static readonly DependencyProperty LatitudeToProperty =
        DependencyProperty.Register("LatitudeTo", typeof(double), typeof(Sphere),
            new PropertyMetadata(-90.0, ModelVisualBase.PropertyChanged), ValidateLatitude);

    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register("Radius", typeof(double), typeof(Sphere),
            new PropertyMetadata(1.0, ModelVisualBase.PropertyChanged), ValidateNonNegative);

    public int Slices
    {
        get => (int)GetValue(SlicesProperty);
        set => SetValue(SlicesProperty, value);
    }

    public int Stacks
    {
        get => (int)GetValue(StacksProperty);
        set => SetValue(StacksProperty, value);
    }

    public Point3D Center
    {
        get => (Point3D)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    public double LongitudeFrom
    {
        get => (double)GetValue(LongitudeFromProperty);
        set => SetValue(LongitudeFromProperty, value);
    }

    public double LongitudeTo
    {
        get => (double)GetValue(LongitudeToProperty);
        set => SetValue(LongitudeFromProperty, value);
    }

    public double LatitudeFrom
    {
        get => (double)GetValue(LatitudeFromProperty);
        set => SetValue(LatitudeFromProperty, value);
    }

    public double LatitudeTo
    {
        get => (double)GetValue(LatitudeToProperty);
        set => SetValue(LatitudeToProperty, value);
    }

    public double Radius
    {
        get => (double)GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    public Sphere()
    {
        rotate = new AxisAngleRotation3D();
        xform = new RotateTransform3D(rotate);
        ModelVisualBase.PropertyChanged(this, default);
    }

    private static bool ValidatePositive(object value)
    {
        return (int)value > 0;
    }

    private static bool ValidateNonNegative(object value)
    {
        return (double)value >= 0.0;
    }

    private static bool ValidateLongitude(object value)
    {
        double num = (double)value;
        if (num >= -180.0)
        {
            return num <= 180.0;
        }
        return false;
    }

    private static bool ValidateLatitude(object value)
    {
        double num = (double)value;
        if (num >= -90.0)
        {
            return num <= 90.0;
        }
        return false;
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        int slices = Slices;
        int stacks = Stacks;
        double radius = Radius;
        Point3D center = Center;
        double num = Math.Max(LatitudeFrom, LatitudeTo);
        double num2 = Math.Min(LatitudeFrom, LatitudeTo);
        double longitudeFrom = LongitudeFrom;
        double longitudeTo = LongitudeTo;
        for (int i = 0; i <= stacks; i++)
        {
            double num3 = num - (double)i * (num - num2) / (double)stacks;
            double num4 = Math.PI * num3 / 180.0;
            double y = radius * Math.Sin(num4);
            double num5 = Math.Cos(num4);
            for (int j = 0; j <= slices; j++)
            {
                double num6 = longitudeTo - longitudeFrom;
                if (num6 < 0.0)
                {
                    num6 += 360.0;
                }
                num3 = longitudeFrom + (double)j * num6 / (double)slices;
                num4 = Math.PI * num3 / 180.0;
                double x = radius * num5 * Math.Sin(num4);
                double z = radius * num5 * Math.Cos(num4);
                Vector3D vector3D = new Vector3D(x, y, z);
                vertices.Add(center + vector3D);
                normals.Add(vector3D);
                textures.Add(new Point((double)j / (double)slices, (double)i / (double)stacks));
            }
        }
        for (int k = 0; k < stacks; k++)
        {
            int num7 = k * (slices + 1);
            int num8 = num7 + slices + 1;
            for (int l = 0; l < slices; l++)
            {
                indices.Add(num7 + l);
                indices.Add(num8 + l);
                indices.Add(num8 + l + 1);
                indices.Add(num7 + l);
                indices.Add(num8 + l + 1);
                indices.Add(num7 + l + 1);
            }
        }
    }
}
