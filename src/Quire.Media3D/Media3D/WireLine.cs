using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class WireLine : WireBase
{
    public static readonly DependencyProperty Point1Property;

    public static readonly DependencyProperty Point2Property;

    public Point3D Point1
    {
        get => (Point3D)GetValue(Point1Property);
        set => SetValue(Point1Property, value);
    }

    public Point3D Point2
    {
        get => (Point3D)GetValue(Point2Property);
        set => SetValue(Point2Property, value);
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines)
    {
        lines.Clear();
        lines.Add(Point1);
        lines.Add(Point2);
    }

    public WireLine()
    {
    }

    static WireLine()
    {
        Point1Property = DependencyProperty.Register("Point1", typeof(Point3D), typeof(WireLine), new PropertyMetadata((object)default(Point3D), new PropertyChangedCallback(WireBase.PropertyChanged)));
        Point2Property = DependencyProperty.Register("Point2", typeof(Point3D), typeof(WireLine), new PropertyMetadata((object)default(Point3D), new PropertyChangedCallback(WireBase.PropertyChanged)));
    }
}
