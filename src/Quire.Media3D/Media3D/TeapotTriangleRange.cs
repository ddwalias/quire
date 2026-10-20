#nullable disable
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace Quire.Media3D;

[TypeConverter(typeof(TeapotTriangleRangeConverter))]
public partial class TeapotTriangleRange : Animatable
{
    public static readonly DependencyProperty BeginProperty = DependencyProperty.Register("Begin", typeof(int), typeof(TeapotTriangleRange), new PropertyMetadata(0));

    public static readonly DependencyProperty EndProperty = DependencyProperty.Register("End", typeof(int), typeof(TeapotTriangleRange), new PropertyMetadata(2255));

    public int Begin
    {
        get => (int)GetValue(BeginProperty);
        set => SetValue(BeginProperty, value);
    }

    public int End
    {
        get => (int)GetValue(EndProperty);
        set => SetValue(EndProperty, value);
    }

    public static TeapotTriangleRange All => new TeapotTriangleRange();

    public static TeapotTriangleRange Pot => new TeapotTriangleRange(0, 1703);

    public static TeapotTriangleRange Body => new TeapotTriangleRange(0, 1127);

    public static TeapotTriangleRange Handle => new TeapotTriangleRange(1128, 1415);

    public static TeapotTriangleRange Spout => new TeapotTriangleRange(1416, 1703);

    public static TeapotTriangleRange Lid => new TeapotTriangleRange(1704, 2255);

    public TeapotTriangleRange()
    {
    }

    public TeapotTriangleRange(int begin, int end)
    {
        Begin = begin;
        End = end;
    }

    public override string ToString()
    {
        return string.Format("{0},{1}", Begin, End);
    }

    protected override Freezable CreateInstanceCore()
    {
        return new SphereMesh();
    }
}
