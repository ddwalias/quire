using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D.Wires;

public partial class WirePoint : WirePointBase
{
    private static readonly Type TypeOfThis;

    public static readonly DependencyProperty CenterProperty;

    private readonly TranslateTransform3D _translate = new TranslateTransform3D();

    private readonly ScaleTransform3D _scale = new ScaleTransform3D();

    public Point3D Center
    {
        get => (Point3D)GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    protected GeometryModel3D Model => Content as GeometryModel3D;

    static WirePoint()
    {
        TypeOfThis = typeof(WirePoint);
        CenterProperty = SphereMesh.CenterProperty.AddOwner(TypeOfThis, new PropertyMetadata(new PropertyChangedCallback(WireBase.PropertyChanged)));
    }

    public WirePoint()
    {
        Model.Geometry = WirePointBase.Mesh.Geometry;
        Transform3DGroup transform3DGroup = new Transform3DGroup();
        transform3DGroup.Children.Add(_scale);
        transform3DGroup.Children.Add(_translate);
        Model.Transform = transform3DGroup;
    }

    protected override void Recalculate()
    {
        NeedRecalculation = false;
        _translate.OffsetX = Center.X;
        _translate.OffsetY = Center.Y;
        _translate.OffsetZ = Center.Z;
        _scale.ScaleX = _scale.ScaleY = _scale.ScaleZ = GetRadiusScale(Center);
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines)
    {
    }
}
