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
                Point3D vertex = new Point3D(x, y, z);
                vertices.Add(vertex);
                normals.Add(vertex - new Point3D(x, y, 0.0));
                textures.Add(new Point((double)n / (double)Slices, (double)m / (double)Stacks));
            }
        }
        for (int stackIndex = 0; stackIndex < Stacks; stackIndex++)
        {
            for (int sliceIndex = 0; sliceIndex < Slices; sliceIndex++)
            {
                indices.Add(num + stackIndex * (Slices + 1) + sliceIndex);
                indices.Add(num + (stackIndex + 1) * (Slices + 1) + sliceIndex);
                indices.Add(num + stackIndex * (Slices + 1) + sliceIndex + 1);
                indices.Add(num + stackIndex * (Slices + 1) + sliceIndex + 1);
                indices.Add(num + (stackIndex + 1) * (Slices + 1) + sliceIndex);
                indices.Add(num + (stackIndex + 1) * (Slices + 1) + sliceIndex + 1);
            }
        }
        num = vertices.Count;
        x = -Width / 2.0;
        for (int stackIndex = 0; stackIndex <= Stacks; stackIndex++)
        {
            y = Height / 2.0 - (double)stackIndex * Height / (double)Stacks;
            for (int layerIndex = 0; layerIndex <= Layers; layerIndex++)
            {
                z = -Depth / 2.0 + (double)layerIndex * Depth / (double)Layers;
                Point3D vertex = new Point3D(x, y, z);
                vertices.Add(vertex);
                normals.Add(vertex - new Point3D(0.0, y, z));
                textures.Add(new Point((double)layerIndex / (double)Layers, (double)stackIndex / (double)Stacks));
            }
        }
        for (int stackIndex = 0; stackIndex < Stacks; stackIndex++)
        {
            for (int layerIndex = 0; layerIndex < Layers; layerIndex++)
            {
                indices.Add(num + stackIndex * (Layers + 1) + layerIndex);
                indices.Add(num + (stackIndex + 1) * (Layers + 1) + layerIndex);
                indices.Add(num + stackIndex * (Layers + 1) + layerIndex + 1);
                indices.Add(num + stackIndex * (Layers + 1) + layerIndex + 1);
                indices.Add(num + (stackIndex + 1) * (Layers + 1) + layerIndex);
                indices.Add(num + (stackIndex + 1) * (Layers + 1) + layerIndex + 1);
            }
        }
        num = vertices.Count;
        x = Width / 2.0;
        for (int stackIndex = 0; stackIndex <= Stacks; stackIndex++)
        {
            y = Height / 2.0 - (double)stackIndex * Height / (double)Stacks;
            for (int layerIndex = 0; layerIndex <= Layers; layerIndex++)
            {
                z = Depth / 2.0 - (double)layerIndex * Depth / (double)Layers;
                Point3D vertex = new Point3D(x, y, z);
                vertices.Add(vertex);
                normals.Add(vertex - new Point3D(0.0, y, z));
                textures.Add(new Point((double)layerIndex / (double)Layers, (double)stackIndex / (double)Stacks));
            }
        }
        for (int stackIndex = 0; stackIndex < Stacks; stackIndex++)
        {
            for (int layerIndex = 0; layerIndex < Layers; layerIndex++)
            {
                indices.Add(num + stackIndex * (Layers + 1) + layerIndex);
                indices.Add(num + (stackIndex + 1) * (Layers + 1) + layerIndex);
                indices.Add(num + stackIndex * (Layers + 1) + layerIndex + 1);
                indices.Add(num + stackIndex * (Layers + 1) + layerIndex + 1);
                indices.Add(num + (stackIndex + 1) * (Layers + 1) + layerIndex);
                indices.Add(num + (stackIndex + 1) * (Layers + 1) + layerIndex + 1);
            }
        }
        num = vertices.Count;
        y = Height / 2.0;
        for (int layerIndex = 0; layerIndex <= Layers; layerIndex++)
        {
            z = -Depth / 2.0 + (double)layerIndex * Depth / (double)Layers;
            for (int sliceIndex = 0; sliceIndex <= Slices; sliceIndex++)
            {
                x = -Width / 2.0 + (double)sliceIndex * Width / (double)Slices;
                Point3D vertex = new Point3D(x, y, z);
                vertices.Add(vertex);
                normals.Add(vertex - new Point3D(x, 0.0, z));
                textures.Add(new Point((double)sliceIndex / (double)Slices, (double)layerIndex / (double)Layers));
            }
        }
        for (int layerIndex = 0; layerIndex < Layers; layerIndex++)
        {
            for (int sliceIndex = 0; sliceIndex < Slices; sliceIndex++)
            {
                indices.Add(num + layerIndex * (Slices + 1) + sliceIndex);
                indices.Add(num + (layerIndex + 1) * (Slices + 1) + sliceIndex);
                indices.Add(num + layerIndex * (Slices + 1) + sliceIndex + 1);
                indices.Add(num + layerIndex * (Slices + 1) + sliceIndex + 1);
                indices.Add(num + (layerIndex + 1) * (Slices + 1) + sliceIndex);
                indices.Add(num + (layerIndex + 1) * (Slices + 1) + sliceIndex + 1);
            }
        }
        num = vertices.Count;
        y = -Height / 2.0;
        for (int layerIndex = 0; layerIndex <= Layers; layerIndex++)
        {
            z = Depth / 2.0 - (double)layerIndex * Depth / (double)Layers;
            for (int sliceIndex = 0; sliceIndex <= Slices; sliceIndex++)
            {
                x = -Width / 2.0 + (double)sliceIndex * Width / (double)Slices;
                Point3D vertex = new Point3D(x, y, z);
                vertices.Add(vertex);
                normals.Add(vertex - new Point3D(x, 0.0, z));
                textures.Add(new Point((double)sliceIndex / (double)Slices, (double)layerIndex / (double)Layers));
            }
        }
        for (int layerIndex = 0; layerIndex < Layers; layerIndex++)
        {
            for (int sliceIndex = 0; sliceIndex < Slices; sliceIndex++)
            {
                indices.Add(num + layerIndex * (Slices + 1) + sliceIndex);
                indices.Add(num + (layerIndex + 1) * (Slices + 1) + sliceIndex);
                indices.Add(num + layerIndex * (Slices + 1) + sliceIndex + 1);
                indices.Add(num + layerIndex * (Slices + 1) + sliceIndex + 1);
                indices.Add(num + (layerIndex + 1) * (Slices + 1) + sliceIndex);
                indices.Add(num + (layerIndex + 1) * (Slices + 1) + sliceIndex + 1);
            }
        }
    }

    protected override Freezable CreateInstanceCore()
    {
        return new BoxMesh();
    }
}
