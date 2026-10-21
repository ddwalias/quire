using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class WireFrame : WireBase
{
    public static readonly DependencyProperty PositionsProperty;
    public static readonly DependencyProperty NormalsProperty;
    public static readonly DependencyProperty TriangleIndicesProperty;
    public static readonly DependencyProperty TextureCoordinatesProperty;

    public Point3DCollection Positions
    {
        get => (Point3DCollection)GetValue(PositionsProperty);
        set => SetValue(PositionsProperty, value);
    }

    public Vector3DCollection Normals
    {
        get => (Vector3DCollection)GetValue(NormalsProperty);
        set => SetValue(NormalsProperty, value);
    }

    public Int32Collection TriangleIndices
    {
        get => (Int32Collection)GetValue(TriangleIndicesProperty);
        set => SetValue(TriangleIndicesProperty, value);
    }

    public PointCollection TextureCoordinates
    {
        get => (PointCollection)GetValue(TextureCoordinatesProperty);
        set => SetValue(TextureCoordinatesProperty, value);
    }

    static WireFrame()
    {
        PositionsProperty = MeshGeometry3D.PositionsProperty.AddOwner(typeof(WireFrame), new PropertyMetadata(WireBase.PropertyChanged));
        NormalsProperty = MeshGeometry3D.NormalsProperty.AddOwner(typeof(WireFrame), new PropertyMetadata(WireBase.PropertyChanged));
        TriangleIndicesProperty = MeshGeometry3D.TriangleIndicesProperty.AddOwner(typeof(WireFrame), new PropertyMetadata(WireBase.PropertyChanged));
        TextureCoordinatesProperty = MeshGeometry3D.TextureCoordinatesProperty.AddOwner(typeof(WireFrame), new PropertyMetadata(WireBase.PropertyChanged));
    }

    public WireFrame()
    {
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines)
    {
        Point3DCollection positions = Positions;
        Int32Collection triangleIndices = TriangleIndices;
        lines.Clear();
        if (positions != null && positions.Count > 0 && triangleIndices != null && triangleIndices.Count > 0)
        {
            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                lines.Add(positions[triangleIndices[i]]);
                lines.Add(positions[triangleIndices[i + 1]]);
                lines.Add(positions[triangleIndices[i + 1]]);
                lines.Add(positions[triangleIndices[i + 2]]);
                lines.Add(positions[triangleIndices[i + 2]]);
                lines.Add(positions[triangleIndices[i]]);
            }
        }
    }
}
