using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace Quire.Media3D.Wires;

public partial class WirePoints : WirePointBase
{
    private static readonly Type _typeOfThis = typeof(WirePoints);

    public static readonly DependencyProperty PositionsProperty =
        DependencyProperty.Register("Positions", typeof(Point3DCollection), _typeOfThis,
            new PropertyMetadata(null, PositionsPropertyChanged));

    private readonly ScaleTransform3D _sphereScaleTransform = new ScaleTransform3D(1.0, 1.0, 1.0);

    public Point3DCollection Positions
    {
        get => (Point3DCollection)GetValue(PositionsProperty);
        set => SetValue(PositionsProperty, value);
    }

    private static void PositionsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        WirePoints wirePoints = sender as WirePoints;
        if (DebugTools.AssertNotNull(wirePoints, null).Passed)
        {
            wirePoints.CreateSpheres();
        }
    }

    protected override void UpdateColor()
    {
        Material material = Tools3D.MaterialForColor(Color);
        Model3DGroup model3DGroup = Content as Model3DGroup;
        if (DebugTools.AssertNotNull(model3DGroup, null).Passed)
        {
            model3DGroup.Children.OfType<GeometryModel3D>().Each(delegate(GeometryModel3D model)
            {
                model.Material = material;
            });
        }
    }

    private void CreateSpheres()
    {
        if (DebugTools.AssertNotNull(Positions, null).Failed)
        {
            return;
        }
        Cursor overrideCursor = Mouse.OverrideCursor;
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            Model3DGroup model3DGroup = new Model3DGroup();
            Material material = Tools3D.MaterialForColor(Color);
            Point3D? point3D = null;
            foreach (Point3D position in Positions)
            {
                if (!point3D.HasValue || !(point3D.Value == position))
                {
                    point3D = position;
                    TranslateTransform3D translateTransform3D = new TranslateTransform3D(position.X, position.Y, position.Z);
                    ((Freezable)translateTransform3D).Freeze();
                    Transform3DGroup transform3DGroup = new Transform3DGroup();
                    transform3DGroup.Children.Add(_sphereScaleTransform);
                    transform3DGroup.Children.Add(translateTransform3D);
                    GeometryModel3D geometryModel3D = new GeometryModel3D(WirePointBase.Mesh.Geometry, material)
                    {
                        Transform = transform3DGroup
                    };
                    model3DGroup.Children.Add(geometryModel3D);
                }
            }
            Content = model3DGroup;
        }
        finally
        {
            Mouse.OverrideCursor = overrideCursor;
        }
    }

    protected override void Recalculate()
    {
        NeedRecalculation = false;
        if (Positions.IsNullOrEmpty())
        {
            return;
        }

        _sphereScaleTransform.ScaleX = _sphereScaleTransform.ScaleY = _sphereScaleTransform.ScaleZ = GetRadiusScale(Positions[0]);
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines)
    {
    }

    public WirePoints()
    {
    }
}
