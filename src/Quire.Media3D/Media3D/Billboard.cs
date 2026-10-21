using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class Billboard : ModelVisualBase
{
    public static readonly DependencyProperty UpperLeftProperty =
        DependencyProperty.Register("UpperLeft", typeof(Point3D), typeof(Billboard),
            new PropertyMetadata(new Point3D(-1.0, 1.0, 0.0), ModelVisualBase.PropertyChanged));

    public static readonly DependencyProperty UpperRightProperty =
        DependencyProperty.Register("UpperRight", typeof(Point3D), typeof(Billboard),
            new PropertyMetadata(new Point3D(1.0, 1.0, 0.0), ModelVisualBase.PropertyChanged));

    public static readonly DependencyProperty LowerLeftProperty =
        DependencyProperty.Register("LowerLeft", typeof(Point3D), typeof(Billboard),
            new PropertyMetadata(new Point3D(-1.0, -1.0, 0.0), ModelVisualBase.PropertyChanged));

    public static readonly DependencyProperty LowerRightProperty =
        DependencyProperty.Register("LowerRight", typeof(Point3D), typeof(Billboard),
            new PropertyMetadata(new Point3D(1.0, -1.0, 0.0), ModelVisualBase.PropertyChanged));

    public static readonly DependencyProperty SlicesProperty =
        DependencyProperty.Register("Slices", typeof(int), typeof(Billboard),
            new PropertyMetadata(1, ModelVisualBase.PropertyChanged), ValidateSlicesAndStacks);

    public static readonly DependencyProperty StacksProperty =
        DependencyProperty.Register("Stacks", typeof(int), typeof(Billboard),
            new PropertyMetadata(1, ModelVisualBase.PropertyChanged), ValidateSlicesAndStacks);

    public Point3D UpperLeft
    {
        get => (Point3D)GetValue(UpperLeftProperty);
        set => SetValue(UpperLeftProperty, value);
    }

    public Point3D UpperRight
    {
        get => (Point3D)GetValue(UpperRightProperty);
        set => SetValue(UpperRightProperty, value);
    }

    public Point3D LowerLeft
    {
        get => (Point3D)GetValue(LowerLeftProperty);
        set => SetValue(LowerLeftProperty, value);
    }

    public Point3D LowerRight
    {
        get => (Point3D)GetValue(LowerRightProperty);
        set => SetValue(LowerRightProperty, value);
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

    public Billboard()
    {
        PropertyChanged(default);
    }

    private static bool ValidateSlicesAndStacks(object value)
    {
        return (int)value > 0;
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        Vector3D vector3D = (Vector3D)UpperLeft;
        Vector3D vector3D2 = (Vector3D)UpperRight;
        Vector3D vector3D3 = (Vector3D)LowerLeft;
        Vector3D vector3D4 = (Vector3D)LowerRight;
        int num = Slices * Stacks;
        Point val = new Point(0.0, 0.0);
        Vector val2 = (new Point(1.0, 0.0) - val) / (double)Slices;
        Vector val3 = (new Point(0.0, 1.0) - val) / (double)Stacks;
        for (int i = 0; i <= Stacks; i++)
        {
            for (int j = 0; j <= Slices; j++)
            {
                vertices.Add((Point3D)(((Stacks - i) * (Slices - j) * vector3D + i * (Slices - j) * vector3D3 + (Stacks - i) * j * vector3D2 + i * j * vector3D4) / num));
                textures.Add(val + (double)i * val3 + (double)j * val2);
                if (j < Slices && i < Stacks)
                {
                    indices.Add((Slices + 1) * i + j);
                    indices.Add((Slices + 1) * (i + 1) + j);
                    indices.Add((Slices + 1) * i + j + 1);
                    indices.Add((Slices + 1) * i + j + 1);
                    indices.Add((Slices + 1) * (i + 1) + j);
                    indices.Add((Slices + 1) * (i + 1) + j + 1);
                }
            }
        }
    }
}
