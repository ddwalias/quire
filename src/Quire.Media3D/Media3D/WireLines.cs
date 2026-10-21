using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class WireLines : WireBase
{
    public static readonly DependencyProperty LinesProperty;

    public Point3DCollection Lines
    {
        get => (Point3DCollection)GetValue(LinesProperty);
        set => SetValue(LinesProperty, value);
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines)
    {
        lines.Clear();
        foreach (Point3D line in Lines)
        {
            lines.Add(line);
        }
    }

    public WireLines()
    {
    }

    static WireLines()
    {
        LinesProperty = DependencyProperty.Register("Lines", typeof(Point3DCollection), typeof(WireLines), new PropertyMetadata(null, new PropertyChangedCallback(WireBase.PropertyChanged)));
    }
}
