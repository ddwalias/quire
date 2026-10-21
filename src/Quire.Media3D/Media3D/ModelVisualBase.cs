using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public abstract partial class ModelVisualBase : ModelVisual3D
{
    private Point3DCollection verticesPreTransform = new Point3DCollection();
    private Vector3DCollection normalsPreTransform = new Vector3DCollection();
    private Point3DCollection normalsAsPoints = new Point3DCollection();
    private WireFrame wireframe;

    public static readonly DependencyProperty NameProperty;
    public static readonly DependencyProperty MaterialProperty;
    public static readonly DependencyProperty BackMaterialProperty;
    private static readonly DependencyPropertyKey GeometryKey;
    public static readonly DependencyProperty GeometryProperty;
    public static readonly DependencyProperty AlgorithmicTransformsProperty;
    public static readonly DependencyProperty IsWireFrameProperty;

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public Material Material
    {
        get => (Material)GetValue(MaterialProperty);
        set => SetValue(MaterialProperty, value);
    }

    public Material BackMaterial
    {
        get => (Material)GetValue(BackMaterialProperty);
        set => SetValue(BackMaterialProperty, value);
    }

    public MeshGeometry3D Geometry
    {
        get => (MeshGeometry3D)GetValue(GeometryProperty);
        protected set => SetValue(GeometryKey, value);
    }

    public AlgorithmicTransformCollection AlgorithmicTransforms
    {
        get => (AlgorithmicTransformCollection)GetValue(AlgorithmicTransformsProperty);
        set => SetValue(AlgorithmicTransformsProperty, value);
    }

    public bool IsWireFrame
    {
        get => (bool)GetValue(IsWireFrameProperty);
        set => SetValue(IsWireFrameProperty, value);
    }

    public ModelVisualBase()
    {
        Geometry = Geometry.Clone();
        GeometryModel3D content = new GeometryModel3D(Geometry, null);
        Content = content;
        AlgorithmicTransforms = new AlgorithmicTransformCollection();
    }

    static ModelVisualBase()
    {
        NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(ModelVisualBase));
        GeometryKey = DependencyProperty.RegisterReadOnly("Geometry", typeof(MeshGeometry3D), typeof(ModelVisualBase), new PropertyMetadata(new MeshGeometry3D()));
        GeometryProperty = GeometryKey.DependencyProperty;
        AlgorithmicTransformsProperty = DependencyProperty.Register("AlgorithmicTransform", typeof(AlgorithmicTransformCollection), typeof(ModelVisualBase), new PropertyMetadata(null, PropertyChanged));
        IsWireFrameProperty = DependencyProperty.Register("IsWireFrame", typeof(bool), typeof(ModelVisualBase), new PropertyMetadata(false, PropertyChanged));
        MaterialProperty = GeometryModel3D.MaterialProperty.AddOwner(typeof(ModelVisualBase), new PropertyMetadata(MaterialPropertyChanged));
        BackMaterialProperty = GeometryModel3D.BackMaterialProperty.AddOwner(typeof(ModelVisualBase), new PropertyMetadata(MaterialPropertyChanged));
    }

    private static void MaterialPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        (obj as ModelVisualBase).MaterialPropertyChanged(args);
    }

    private void MaterialPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        GeometryModel3D geometryModel3D = Content as GeometryModel3D;
        if (args.Property == MaterialProperty)
        {
            geometryModel3D.Material = args.NewValue as Material;
        }
        else if (args.Property == BackMaterialProperty)
        {
            geometryModel3D.BackMaterial = args.NewValue as Material;
        }
    }

    protected static void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        (obj as ModelVisualBase).PropertyChanged(args);
    }

    protected virtual void PropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        Point3DCollection positions;
        Vector3DCollection normals;
        Int32Collection triangleIndices;
        PointCollection textureCoordinates;
        if (!IsWireFrame || (args.Property == IsWireFrameProperty && !(bool)args.OldValue))
        {
            MeshGeometry3D geometry = Geometry;
            positions = geometry.Positions;
            normals = geometry.Normals;
            triangleIndices = geometry.TriangleIndices;
            textureCoordinates = geometry.TextureCoordinates;
            geometry.Positions = null;
            geometry.Normals = null;
            geometry.TriangleIndices = null;
            geometry.TextureCoordinates = null;
        }
        else
        {
            positions = wireframe.Positions;
            normals = wireframe.Normals;
            triangleIndices = wireframe.TriangleIndices;
            textureCoordinates = wireframe.TextureCoordinates;
            wireframe.Positions = null;
            wireframe.Normals = null;
            wireframe.TriangleIndices = null;
            wireframe.TextureCoordinates = null;
        }
        if (args.Property != AlgorithmicTransformsProperty && args.Property != IsWireFrameProperty)
        {
            Triangulate(args, positions, normals, triangleIndices, textureCoordinates);
            verticesPreTransform.Clear();
            normalsPreTransform.Clear();
            foreach (Point3D item in positions)
            {
                verticesPreTransform.Add(item);
            }
            foreach (Vector3D item2 in normals)
            {
                normalsPreTransform.Add(item2);
            }
        }
        if (args.Property == AlgorithmicTransformsProperty)
        {
            positions.Clear();
            normals.Clear();
            normalsAsPoints.Clear();
            foreach (Point3D item3 in verticesPreTransform)
            {
                positions.Add(item3);
            }
            foreach (Vector3D item4 in normalsPreTransform)
            {
                normalsAsPoints.Add((Point3D)item4);
            }
        }
        if (args.Property != IsWireFrameProperty)
        {
            foreach (AlgorithmicTransform algorithmicTransform in AlgorithmicTransforms)
            {
                algorithmicTransform.Transform(positions);
                algorithmicTransform.Transform(normalsAsPoints);
            }
            foreach (Point3D normalsAsPoint in normalsAsPoints)
            {
                normals.Add((Vector3D)normalsAsPoint);
            }
        }
        if (IsWireFrame)
        {
            if (wireframe == null)
            {
                wireframe = new WireFrame();
                Children.Add(wireframe);
            }
            wireframe.TextureCoordinates = textureCoordinates;
            wireframe.TriangleIndices = triangleIndices;
            wireframe.Normals = normals;
            wireframe.Positions = positions;
        }
        else
        {
            MeshGeometry3D geometry2 = Geometry;
            geometry2.TextureCoordinates = textureCoordinates;
            geometry2.TriangleIndices = triangleIndices;
            geometry2.Normals = normals;
            geometry2.Positions = positions;
        }
    }

    protected abstract void Triangulate(DependencyPropertyChangedEventArgs args, Point3DCollection vertices, Vector3DCollection normals, Int32Collection indices, PointCollection textures);
}
