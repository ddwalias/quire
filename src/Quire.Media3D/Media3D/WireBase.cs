using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire.Media2D;

namespace Quire.Media3D;

public abstract partial class WireBase : ModelVisual3D, IDisposable
{
    protected Matrix3D matxVisualToScreen = Matrix3D.Identity;
    protected Matrix3D matxScreenToVisual = Matrix3D.Identity;
    private readonly RotateTransform rotate = new RotateTransform();
    private bool _needRecalculation = true;

    private static readonly DependencyPropertyKey LineCollectionKey =
        DependencyProperty.RegisterReadOnly("LineCollection", typeof(Point3DCollection), typeof(WireBase),
            new PropertyMetadata(null, RecalcPropertyChanged));

    public static readonly DependencyProperty LineCollectionProperty = LineCollectionKey.DependencyProperty;

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(Color), typeof(WireBase),
            new PropertyMetadata(Colors.Black, ColorPropertyChanged));

    public static readonly DependencyProperty ThicknessProperty =
        DependencyProperty.Register("Thickness", typeof(double), typeof(WireBase),
            new PropertyMetadata(1.0, ThicknessChanged));

    public static readonly DependencyProperty RoundingProperty =
        DependencyProperty.Register("Rounding", typeof(int), typeof(WireBase),
            new PropertyMetadata(0, RecalcPropertyChanged));

    public static readonly DependencyProperty ArrowAngleProperty =
        DependencyProperty.Register("ArrowAngle", typeof(double), typeof(WireBase),
            new PropertyMetadata(45.0, RecalcPropertyChanged));

    public static readonly DependencyProperty ArrowLengthProperty =
        DependencyProperty.Register("ArrowLength", typeof(double), typeof(WireBase),
            new PropertyMetadata(12.0, RecalcPropertyChanged));

    public static readonly DependencyProperty ArrowEndsProperty =
        DependencyProperty.Register("ArrowEnds", typeof(ArrowEnds), typeof(WireBase),
            new PropertyMetadata(ArrowEnds.None, RecalcPropertyChanged));

    protected bool NeedRecalculation
    {
        get => _needRecalculation;
        set => _needRecalculation = value;
    }

    public Point3DCollection LineCollection
    {
        get => (Point3DCollection)GetValue(LineCollectionProperty);
        private set => SetValue(LineCollectionKey, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public double Thickness
    {
        get => (double)GetValue(ThicknessProperty);
        set => SetValue(ThicknessProperty, value);
    }

    public int Rounding
    {
        get => (int)GetValue(RoundingProperty);
        set => SetValue(RoundingProperty, value);
    }

    public double ArrowAngle
    {
        get => (double)GetValue(ArrowAngleProperty);
        set => SetValue(ArrowAngleProperty, value);
    }

    public double ArrowLength
    {
        get => (double)GetValue(ArrowLengthProperty);
        set => SetValue(ArrowLengthProperty, value);
    }

    public ArrowEnds ArrowEnds
    {
        get => (ArrowEnds)GetValue(ArrowEndsProperty);
        set => SetValue(ArrowEndsProperty, value);
    }

    protected WireBase()
    {
        LineCollection = new Point3DCollection();
        MeshGeometry3D geometry = new MeshGeometry3D();
        Material material = Tools3D.MaterialForColor(Color);
        GeometryModel3D geometryModel3D = new GeometryModel3D(geometry, material);
        geometryModel3D.BackMaterial = material;
        Content = geometryModel3D;
    }

    protected static void ColorPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        (obj as WireBase).UpdateColor();
    }

    protected virtual void UpdateColor()
    {
        (Content as GeometryModel3D).Material = Tools3D.MaterialForColor(Color);
    }

    private static void ThicknessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        WireBase obj2 = obj as WireBase;
        obj2.NeedRecalculation = true;
        obj2.Recalculate();
    }

    private static void RecalcPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        (obj as WireBase).NeedRecalculation = true;
    }

    protected static void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        (obj as WireBase).PropertyChanged(args);
    }

    protected void PropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        Point3DCollection lineCollection = LineCollection;
        LineCollection = null;
        Generate(args, lineCollection);
        LineCollection = lineCollection;
    }

    protected abstract void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines);

    internal virtual void OnRendering(Matrix3D matx)
    {
        if (matx == VisualInfo.ZeroMatrix)
        {
            return;
        }
        if (matx.IsIdentity)
        {
            return;
        }
        if (matx != matxVisualToScreen)
        {
            matxVisualToScreen = matx;
            matxScreenToVisual = matx;
            if (matxScreenToVisual.HasInverse)
            {
                matxScreenToVisual.Invert();
                NeedRecalculation = true;
            }
            else
            {
                DebugTools.DevAssert(false, null);
            }
        }
        if (NeedRecalculation)
        {
            Recalculate();
        }
    }

    protected virtual void Recalculate()
    {
        NeedRecalculation = false;
        Point3DCollection lineCollection = LineCollection;
        MeshGeometry3D meshGeometry3D = (Content as GeometryModel3D).Geometry as MeshGeometry3D;
        int count = lineCollection.Count;
        Point3DCollection point3DCollection = new Point3DCollection(count * 2);
        Int32Collection int32Collection = new Int32Collection(count * 3);
        int indicesBase = 0;
        for (int i = 0; i < count - 1; i += 2)
        {
            Point3D point3D = lineCollection[i];
            Point3D point3D2 = lineCollection[i + 1];
            DoLine(point3D, point3D2, point3DCollection, int32Collection, ref indicesBase);
            if (i == 0 && (ArrowEnds & ArrowEnds.Start) == ArrowEnds.Start)
            {
                DoArrow(point3D2, point3D, point3DCollection, int32Collection, ref indicesBase);
            }
            if (i > lineCollection.Count - 4 && (ArrowEnds & ArrowEnds.End) == ArrowEnds.End)
            {
                DoArrow(point3D, point3D2, point3DCollection, int32Collection, ref indicesBase);
            }
        }
        meshGeometry3D.TriangleIndices = int32Collection;
        meshGeometry3D.Positions = point3DCollection;
    }

    private void DoArrow(Point3D pt1, Point3D pt2, Point3DCollection points, Int32Collection indices, ref int indicesBase)
    {
        Point3D point3D = pt1 * matxVisualToScreen;
        Point3D point3D2 = pt2 * matxVisualToScreen;
        Vector val = new Vector(point3D.X - point3D2.X, point3D.Y - point3D2.Y);
        val.Normalize();
        val *= ArrowLength;
        Matrix val2 = default(Matrix);
        val2.Rotate(ArrowAngle / 2.0);
        Point3D pt3 = Widen(pt2, val * val2);
        val2.Rotate(-ArrowAngle);
        Point3D pt4 = Widen(pt2, val * val2);
        DoLine(pt2, pt3, points, indices, ref indicesBase);
        DoLine(pt2, pt4, points, indices, ref indicesBase);
    }

    private void DoLine(Point3D pt1, Point3D pt2, Point3DCollection points, Int32Collection indices, ref int indicesBase)
    {
        Point3D point3D = pt1 * matxVisualToScreen;
        Vector3D vector3D = pt2 * matxVisualToScreen - point3D;
        vector3D.Z = 0.0;
        vector3D.Normalize();
        Vector val = Thickness / 2.0 * new Vector(-vector3D.Y, vector3D.X);
        points.Add(Widen(pt1, val));
        points.Add(Widen(pt1, -val));
        points.Add(Widen(pt2, val));
        points.Add(Widen(pt2, -val));
        indices.Add(indicesBase);
        indices.Add(indicesBase + 2);
        indices.Add(indicesBase + 1);
        indices.Add(indicesBase + 1);
        indices.Add(indicesBase + 2);
        indices.Add(indicesBase + 3);
        indicesBase += 4;
        if (Rounding > 0)
        {
            AddRounding(pt1, val, points, indices, ref indicesBase);
            AddRounding(pt2, -val, points, indices, ref indicesBase);
        }
    }

    protected Point3D Widen(Point3D pointIn, Vector delta)
    {
        Point4D point4D = (Point4D)pointIn * matxVisualToScreen;
        point4D.X += delta.X * point4D.W;
        point4D.Y += delta.Y * point4D.W;
        point4D *= matxScreenToVisual;
        return new Point3D(point4D.X / point4D.W, point4D.Y / point4D.W, point4D.Z / point4D.W);
    }

    private void AddRounding(Point3D ptIn, Vector delta, Point3DCollection points, Int32Collection indices, ref int indicesCount)
    {
        points.Add(CalculatePoint(ptIn, new Vector(0.0, 0.0), 0.0));
        for (int i = 0; i <= Rounding; i++)
        {
            points.Add(CalculatePoint(ptIn, delta, 180 * i / Rounding));
        }
        for (int j = 0; j < Rounding; j++)
        {
            indices.Add(indicesCount);
            indices.Add(indicesCount + j + 2);
            indices.Add(indicesCount + j + 1);
        }
        indicesCount += Rounding + 2;
    }

    private Point3D CalculatePoint(Point3D ptIn, Vector delta, double angle)
    {
        Point4D point4D = (Point4D)ptIn * matxVisualToScreen;
        rotate.Angle = angle;
        delta = (Vector)rotate.Transform((Point)delta);
        point4D.X += delta.X * point4D.W;
        point4D.Y += delta.Y * point4D.W;
        point4D *= matxScreenToVisual;
        return new Point3D(point4D.X / point4D.W, point4D.Y / point4D.W, point4D.Z / point4D.W);
    }

    public void Dispose()
    {
        foreach (IDisposable item in Children.OfType<IDisposable>())
        {
            item.Dispose();
        }
    }
}
