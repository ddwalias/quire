#nullable disable
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

[RuntimeNameProperty("Name")]
public abstract partial class MeshGeneratorBase : Animatable
{
    public static readonly DependencyProperty NameProperty;
    private static DependencyPropertyKey GeometryKey;
    public static readonly DependencyProperty GeometryProperty;

    static MeshGeneratorBase()
    {
        NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(MeshGeneratorBase));
        GeometryKey = DependencyProperty.RegisterReadOnly("Geometry", typeof(MeshGeometry3D), typeof(MeshGeneratorBase), new PropertyMetadata((object)new MeshGeometry3D()));
        GeometryProperty = GeometryKey.DependencyProperty;
    }

    public MeshGeneratorBase()
    {
        Geometry = new MeshGeometry3D();
    }

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public MeshGeometry3D Geometry
    {
        get => (MeshGeometry3D)GetValue(GeometryProperty);
        protected set => SetValue(GeometryKey, value);
    }

    protected static void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        (obj as MeshGeneratorBase).PropertyChanged(args);
    }

    protected virtual void PropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        MeshGeometry3D geometry = Geometry;
        Point3DCollection positions = geometry.Positions;
        Vector3DCollection normals = geometry.Normals;
        Int32Collection triangleIndices = geometry.TriangleIndices;
        PointCollection textureCoordinates = geometry.TextureCoordinates;
        geometry.Positions = null;
        geometry.Normals = null;
        geometry.TriangleIndices = null;
        geometry.TextureCoordinates = null;
        Triangulate(args, positions, normals, triangleIndices, textureCoordinates);
        geometry.TextureCoordinates = textureCoordinates;
        geometry.TriangleIndices = triangleIndices;
        geometry.Normals = normals;
        geometry.Positions = positions;
    }

    protected abstract void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures);
}
