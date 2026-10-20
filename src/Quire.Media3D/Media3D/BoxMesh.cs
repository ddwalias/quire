#nullable disable
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class BoxMesh : MeshGeneratorBase
{
    public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(BoxMesh), new PropertyMetadata(1.0, PropertyChanged));

    public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(BoxMesh), new PropertyMetadata(1.0, PropertyChanged));

    public static readonly DependencyProperty DepthProperty = DependencyProperty.Register("Depth", typeof(double), typeof(BoxMesh), new PropertyMetadata(1.0, PropertyChanged));

    public static readonly DependencyProperty SlicesProperty = DependencyProperty.Register("Slices", typeof(int), typeof(BoxMesh), new PropertyMetadata(1, PropertyChanged), ValidateDivisions);

    public static readonly DependencyProperty StacksProperty = DependencyProperty.Register("Stacks", typeof(int), typeof(BoxMesh), new PropertyMetadata(1, PropertyChanged), ValidateDivisions);

    public static readonly DependencyProperty LayersProperty = DependencyProperty.Register("Layers", typeof(int), typeof(BoxMesh), new PropertyMetadata(1, PropertyChanged), ValidateDivisions);

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

    public int Layers
    {
        get => (int)GetValue(LayersProperty);
        set => SetValue(LayersProperty, value);
    }

    public BoxMesh()
    {
        PropertyChanged(default(DependencyPropertyChangedEventArgs));
    }

    private static bool ValidateDivisions(object obj)
    {
        return (int)obj > 0;
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        double x;
        double y;
        double z;
        int num = 0;
        z = Depth / 2.0;
        for (int i = 0; i <= Stacks; i++)
        {
            y = Height / 2.0 - (double)i * Height / (double)Stacks;
            for (int j = 0; j <= Slices; j++)
            {
                x = -Width / 2.0 + (double)j * Width / (double)Slices;
                Point3D point3D = new Point3D(x, y, z);
                vertices.Add(point3D);
                normals.Add(point3D - new Point3D(x, y, 0.0));
                textures.Add(new Point((double)j / (double)Slices, (double)i / (double)Stacks));
            }
        }
        for (int k = 0; k < Stacks; k++)
        {
            for (int l = 0; l < Slices; l++)
            {
                indices.Add(k * (Slices + 1) + l);
                indices.Add((k + 1) * (Slices + 1) + l);
                indices.Add(k * (Slices + 1) + l + 1);
                indices.Add(k * (Slices + 1) + l + 1);
                indices.Add((k + 1) * (Slices + 1) + l);
                indices.Add((k + 1) * (Slices + 1) + l + 1);
            }
        }
        num = vertices.Count;
        z = -Depth / 2.0;
        for (int m = 0; m <= Stacks; m++)
        {
            y = Height / 2.0 - (double)m * Height / (double)Stacks;
            for (int n = 0; n <= Slices; n++)
            {
                x = Width / 2.0 - (double)n * Width / (double)Slices;
                Point3D point3D2 = new Point3D(x, y, z);
                vertices.Add(point3D2);
                normals.Add(point3D2 - new Point3D(x, y, 0.0));
                textures.Add(new Point((double)n / (double)Slices, (double)m / (double)Stacks));
            }
        }
        for (int num2 = 0; num2 < Stacks; num2++)
        {
            for (int num3 = 0; num3 < Slices; num3++)
            {
                indices.Add(num + num2 * (Slices + 1) + num3);
                indices.Add(num + (num2 + 1) * (Slices + 1) + num3);
                indices.Add(num + num2 * (Slices + 1) + num3 + 1);
                indices.Add(num + num2 * (Slices + 1) + num3 + 1);
                indices.Add(num + (num2 + 1) * (Slices + 1) + num3);
                indices.Add(num + (num2 + 1) * (Slices + 1) + num3 + 1);
            }
        }
        num = vertices.Count;
        x = -Width / 2.0;
        for (int num4 = 0; num4 <= Stacks; num4++)
        {
            y = Height / 2.0 - (double)num4 * Height / (double)Stacks;
            for (int num5 = 0; num5 <= Layers; num5++)
            {
                z = -Depth / 2.0 + (double)num5 * Depth / (double)Layers;
                Point3D point3D3 = new Point3D(x, y, z);
                vertices.Add(point3D3);
                normals.Add(point3D3 - new Point3D(0.0, y, z));
                textures.Add(new Point((double)num5 / (double)Layers, (double)num4 / (double)Stacks));
            }
        }
        for (int num6 = 0; num6 < Stacks; num6++)
        {
            for (int num7 = 0; num7 < Layers; num7++)
            {
                indices.Add(num + num6 * (Layers + 1) + num7);
                indices.Add(num + (num6 + 1) * (Layers + 1) + num7);
                indices.Add(num + num6 * (Layers + 1) + num7 + 1);
                indices.Add(num + num6 * (Layers + 1) + num7 + 1);
                indices.Add(num + (num6 + 1) * (Layers + 1) + num7);
                indices.Add(num + (num6 + 1) * (Layers + 1) + num7 + 1);
            }
        }
        num = vertices.Count;
        x = Width / 2.0;
        for (int num8 = 0; num8 <= Stacks; num8++)
        {
            y = Height / 2.0 - (double)num8 * Height / (double)Stacks;
            for (int num9 = 0; num9 <= Layers; num9++)
            {
                z = Depth / 2.0 - (double)num9 * Depth / (double)Layers;
                Point3D point3D4 = new Point3D(x, y, z);
                vertices.Add(point3D4);
                normals.Add(point3D4 - new Point3D(0.0, y, z));
                textures.Add(new Point((double)num9 / (double)Layers, (double)num8 / (double)Stacks));
            }
        }
        for (int num10 = 0; num10 < Stacks; num10++)
        {
            for (int num11 = 0; num11 < Layers; num11++)
            {
                indices.Add(num + num10 * (Layers + 1) + num11);
                indices.Add(num + (num10 + 1) * (Layers + 1) + num11);
                indices.Add(num + num10 * (Layers + 1) + num11 + 1);
                indices.Add(num + num10 * (Layers + 1) + num11 + 1);
                indices.Add(num + (num10 + 1) * (Layers + 1) + num11);
                indices.Add(num + (num10 + 1) * (Layers + 1) + num11 + 1);
            }
        }
        num = vertices.Count;
        y = Height / 2.0;
        for (int num12 = 0; num12 <= Layers; num12++)
        {
            z = -Depth / 2.0 + (double)num12 * Depth / (double)Layers;
            for (int num13 = 0; num13 <= Slices; num13++)
            {
                x = -Width / 2.0 + (double)num13 * Width / (double)Slices;
                Point3D point3D5 = new Point3D(x, y, z);
                vertices.Add(point3D5);
                normals.Add(point3D5 - new Point3D(x, 0.0, z));
                textures.Add(new Point((double)num13 / (double)Slices, (double)num12 / (double)Layers));
            }
        }
        for (int num14 = 0; num14 < Layers; num14++)
        {
            for (int num15 = 0; num15 < Slices; num15++)
            {
                indices.Add(num + num14 * (Slices + 1) + num15);
                indices.Add(num + (num14 + 1) * (Slices + 1) + num15);
                indices.Add(num + num14 * (Slices + 1) + num15 + 1);
                indices.Add(num + num14 * (Slices + 1) + num15 + 1);
                indices.Add(num + (num14 + 1) * (Slices + 1) + num15);
                indices.Add(num + (num14 + 1) * (Slices + 1) + num15 + 1);
            }
        }
        num = vertices.Count;
        y = -Height / 2.0;
        for (int num16 = 0; num16 <= Layers; num16++)
        {
            z = Depth / 2.0 - (double)num16 * Depth / (double)Layers;
            for (int num17 = 0; num17 <= Slices; num17++)
            {
                x = -Width / 2.0 + (double)num17 * Width / (double)Slices;
                Point3D point3D6 = new Point3D(x, y, z);
                vertices.Add(point3D6);
                normals.Add(point3D6 - new Point3D(x, 0.0, z));
                textures.Add(new Point((double)num17 / (double)Slices, (double)num16 / (double)Layers));
            }
        }
        for (int num18 = 0; num18 < Layers; num18++)
        {
            for (int num19 = 0; num19 < Slices; num19++)
            {
                indices.Add(num + num18 * (Slices + 1) + num19);
                indices.Add(num + (num18 + 1) * (Slices + 1) + num19);
                indices.Add(num + num18 * (Slices + 1) + num19 + 1);
                indices.Add(num + num18 * (Slices + 1) + num19 + 1);
                indices.Add(num + (num18 + 1) * (Slices + 1) + num19);
                indices.Add(num + (num18 + 1) * (Slices + 1) + num19 + 1);
            }
        }
    }

    protected override Freezable CreateInstanceCore()
    {
        return new BoxMesh();
    }
}
