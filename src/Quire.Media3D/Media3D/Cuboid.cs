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
            double rowY = Origin.Y + Height - (double)k * Height / (double)Stacks;
            for (int l = 0; l <= Slices; l++)
            {
                double columnX = Origin.X + Width - (double)l * Width / (double)Slices;
                vertices.Add(new Point3D(columnX, rowY, Origin.Z));
            }
        }
        for (int m = 0; m <= Stacks; m++)
        {
            double rowY = Origin.Y + Height - (double)m * Height / (double)Stacks;
            for (int n = 0; n <= Slivers; n++)
            {
                double z = Origin.Z + (double)n * Depth / (double)Slivers;
                vertices.Add(new Point3D(Origin.X, rowY, z));
            }
        }
        for (int num = 0; num <= Stacks; num++)
        {
            double rowY = Origin.Y + Height - (double)num * Height / (double)Stacks;
            for (int depthIndex = 0; depthIndex <= Slivers; depthIndex++)
            {
                double depth = Origin.Z + Depth - (double)depthIndex * Depth / (double)Slivers;
                vertices.Add(new Point3D(Origin.X + Width, rowY, depth));
            }
        }
        for (int depthIndex = 0; depthIndex <= Slivers; depthIndex++)
        {
            double depth = Origin.Z + (double)depthIndex * Depth / (double)Slivers;
            for (int sliceIndex = 0; sliceIndex <= Slices; sliceIndex++)
            {
                double x = Origin.X + (double)sliceIndex * Width / (double)Slices;
                vertices.Add(new Point3D(x, Origin.Y + Height, depth));
            }
        }
        for (int depthIndex = 0; depthIndex <= Slivers; depthIndex++)
        {
            double depth = Origin.Z + Depth - (double)depthIndex * Depth / (double)Slivers;
            for (int sliceIndex = 0; sliceIndex <= Slices; sliceIndex++)
            {
                double x = Origin.X + (double)sliceIndex * Width / (double)Slices;
                vertices.Add(new Point3D(x, Origin.Y, depth));
            }
        }
        for (int faceIndex = 0; faceIndex < 6; faceIndex++)
        {
            for (int stackIndex = 0; stackIndex <= Stacks; stackIndex++)
            {
                double textureV = (double)stackIndex / (double)Stacks;
                for (int sliceIndex = 0; sliceIndex <= Slices; sliceIndex++)
                {
                    double textureU = (double)sliceIndex / (double)Slices;
                    textures.Add(new Point(textureU, textureV));
                }
            }
        }
        for (int faceIndex = 0; faceIndex < 6; faceIndex++)
        {
            for (int stackIndex = 0; stackIndex < Stacks; stackIndex++)
            {
                for (int sliceIndex = 0; sliceIndex < Slices; sliceIndex++)
                {
                    indices.Add(faceIndex * (Slices + 1) * (Stacks + 1) + stackIndex * (Slices + 1) + sliceIndex);
                    indices.Add(faceIndex * (Slices + 1) * (Stacks + 1) + (stackIndex + 1) * (Slices + 1) + sliceIndex);
                    indices.Add(faceIndex * (Slices + 1) * (Stacks + 1) + stackIndex * (Slices + 1) + sliceIndex + 1);
                    indices.Add(faceIndex * (Slices + 1) * (Stacks + 1) + stackIndex * (Slices + 1) + sliceIndex + 1);
                    indices.Add(faceIndex * (Slices + 1) * (Stacks + 1) + (stackIndex + 1) * (Slices + 1) + sliceIndex);
                    indices.Add(faceIndex * (Slices + 1) * (Stacks + 1) + (stackIndex + 1) * (Slices + 1) + sliceIndex + 1);
                }
            }
        }
    }
}
