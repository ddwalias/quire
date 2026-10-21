using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class ModelVisual : ModelVisualBase
{
    public static readonly DependencyProperty MeshGeneratorProperty =
        DependencyProperty.Register("MeshGeometry", typeof(MeshGeneratorBase), typeof(ModelVisual),
            new PropertyMetadata(null, ModelVisualBase.PropertyChanged));

    public MeshGeneratorBase MeshGenerator
    {
        get => (MeshGeneratorBase)GetValue(MeshGeneratorProperty);
        set => SetValue(MeshGeneratorProperty, value);
    }

    protected override void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures)
    {
        vertices.Clear();
        normals.Clear();
        indices.Clear();
        textures.Clear();
        MeshGeometry3D geometry = MeshGenerator.Geometry;
        foreach (Point3D position in geometry.Positions)
        {
            vertices.Add(position);
        }
        foreach (Vector3D normal in geometry.Normals)
        {
            normals.Add(normal);
        }
        foreach (int triangleIndex in geometry.TriangleIndices)
        {
            indices.Add(triangleIndex);
        }
        foreach (Point textureCoordinate in geometry.TextureCoordinates)
        {
            textures.Add(textureCoordinate);
        }
    }

    public ModelVisual()
    {
    }
}
