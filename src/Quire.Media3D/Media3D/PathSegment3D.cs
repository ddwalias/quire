#nullable disable
using System.Windows;
using System.Windows.Media.Animation;

namespace Quire.Media3D;

public abstract partial class PathSegment3D : Animatable
{
    public static readonly DependencyProperty IsStrokedProperty = DependencyProperty.Register(
        "IsStroked",
        typeof(bool),
        typeof(PathSegment3D),
        new PropertyMetadata(true));

    public static readonly DependencyProperty IsSmoothJoinProperty = DependencyProperty.Register(
        "IsSmoothJoin",
        typeof(bool),
        typeof(PathSegment3D),
        new PropertyMetadata(false));

    protected PathSegment3D()
    {
    }

    public bool IsStroked
    {
        get => (bool)GetValue(IsStrokedProperty);
        set => SetValue(IsStrokedProperty, value);
    }

    public bool IsSmoothJoin
    {
        get => (bool)GetValue(IsSmoothJoinProperty);
        set => SetValue(IsSmoothJoinProperty, value);
    }
}
