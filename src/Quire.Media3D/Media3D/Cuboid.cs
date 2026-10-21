using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class Cuboid : ModelVisualBase
{
    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register("Width", typeof(double), typeof(Cuboid),
            new PropertyMetadata(1.0, ModelVisualBase.PropertyChanged));

    public static readonly DependencyProperty HeightProperty =
        DependencyProperty.Register("Height", typeof(double), typeof(Cuboid),
            new PropertyMetadata(1.0, ModelVisualBase.PropertyChanged));

    public static readonly DependencyProperty DepthProperty =
        DependencyProperty.Register("Depth", typeof(double), typeof(Cuboid),
            new PropertyMetadata(1.0, ModelVisualBase.PropertyChanged));

    public static readonly DependencyProperty OriginProperty =
        DependencyProperty.Register("Origin", typeof(Point3D), typeof(Cuboid),
            new PropertyMetadata(new Point3D(-0.5, -0.5, -0.5), ModelVisualBase.PropertyChanged));

    public static readonly DependencyProperty SlicesProperty =
        DependencyProperty.Register("Slices", typeof(int), typeof(Cuboid),
            new PropertyMetadata(10, ModelVisualBase.PropertyChanged), ValidateSlices);

    public static readonly DependencyProperty StacksProperty =
        DependencyProperty.Register("Stacks", typeof(int), typeof(Cuboid),
            new PropertyMetadata(10, ModelVisualBase.PropertyChanged), ValidateSlices);

    public static readonly DependencyProperty SliversProperty =
        DependencyProperty.Register("Slivers", typeof(int), typeof(Cuboid),
            new PropertyMetadata(10, ModelVisualBase.PropertyChanged), ValidateSlices);

    public double Width
    {
        get => (double)GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    public double Height
    {
        get => (double)GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }

    public double Depth
    {
        get => (double)GetValue(DepthProperty);
        set => SetValue(DepthProperty, value);
    }

    public Point3D Origin
    {
        get => (Point3D)GetValue(OriginProperty);
        set => SetValue(OriginProperty, value);
    }

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

    public int Slivers
    {
        get => (int)GetValue(SliversProperty);
        set => SetValue(SliversProperty, value);
    }

    public Cuboid()
    {
        ModelVisualBase.PropertyChanged(this, default);
    }

    private static bool ValidateSlices(object obj)
    {
        return (int)obj > 0;
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        for (int i = 0; i <= Stacks; i++)
        {
            double y = Origin.Y + Height - (double)i * Height / (double)Stacks;
            for (int j = 0; j <= Slices; j++)
            {
                double x = Origin.X + (double)j * Width / (double)Slices;
                vertices.Add(new Point3D(x, y, Origin.Z + Depth));
            }
        }
        for (int k = 0; k <= Stacks; k++)
        {
            double y2 = Origin.Y + Height - (double)k * Height / (double)Stacks;
            for (int l = 0; l <= Slices; l++)
            {
                double x2 = Origin.X + Width - (double)l * Width / (double)Slices;
                vertices.Add(new Point3D(x2, y2, Origin.Z));
            }
        }
        for (int m = 0; m <= Stacks; m++)
        {
            double y3 = Origin.Y + Height - (double)m * Height / (double)Stacks;
            for (int n = 0; n <= Slivers; n++)
            {
                double z = Origin.Z + (double)n * Depth / (double)Slivers;
                vertices.Add(new Point3D(Origin.X, y3, z));
            }
        }
        for (int num = 0; num <= Stacks; num++)
        {
            double y4 = Origin.Y + Height - (double)num * Height / (double)Stacks;
            for (int num2 = 0; num2 <= Slivers; num2++)
            {
                double z2 = Origin.Z + Depth - (double)num2 * Depth / (double)Slivers;
                vertices.Add(new Point3D(Origin.X + Width, y4, z2));
            }
        }
        for (int num3 = 0; num3 <= Slivers; num3++)
        {
            double z3 = Origin.Z + (double)num3 * Depth / (double)Slivers;
            for (int num4 = 0; num4 <= Slices; num4++)
            {
                double x3 = Origin.X + (double)num4 * Width / (double)Slices;
                vertices.Add(new Point3D(x3, Origin.Y + Height, z3));
            }
        }
        for (int num5 = 0; num5 <= Slivers; num5++)
        {
            double z4 = Origin.Z + Depth - (double)num5 * Depth / (double)Slivers;
            for (int num6 = 0; num6 <= Slices; num6++)
            {
                double x4 = Origin.X + (double)num6 * Width / (double)Slices;
                vertices.Add(new Point3D(x4, Origin.Y, z4));
            }
        }
        for (int num7 = 0; num7 < 6; num7++)
        {
            for (int num8 = 0; num8 <= Stacks; num8++)
            {
                double num9 = (double)num8 / (double)Stacks;
                for (int num10 = 0; num10 <= Slices; num10++)
                {
                    double num11 = (double)num10 / (double)Slices;
                    textures.Add(new Point(num11, num9));
                }
            }
        }
        for (int num12 = 0; num12 < 6; num12++)
        {
            for (int num13 = 0; num13 < Stacks; num13++)
            {
                for (int num14 = 0; num14 < Slices; num14++)
                {
                    indices.Add(num12 * (Slices + 1) * (Stacks + 1) + num13 * (Slices + 1) + num14);
                    indices.Add(num12 * (Slices + 1) * (Stacks + 1) + (num13 + 1) * (Slices + 1) + num14);
                    indices.Add(num12 * (Slices + 1) * (Stacks + 1) + num13 * (Slices + 1) + num14 + 1);
                    indices.Add(num12 * (Slices + 1) * (Stacks + 1) + num13 * (Slices + 1) + num14 + 1);
                    indices.Add(num12 * (Slices + 1) * (Stacks + 1) + (num13 + 1) * (Slices + 1) + num14);
                    indices.Add(num12 * (Slices + 1) * (Stacks + 1) + (num13 + 1) * (Slices + 1) + num14 + 1);
                }
            }
        }
    }
}
